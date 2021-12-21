using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Heracles.Application.Entities;
using Heracles.Application.Interfaces;
using Heracles.Application.Services.Import;
using Heracles.Application.TrackAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Infrastructure.Data
{
    public class TrackRepository : EfRepository<Track, Guid>, ITrackRepository
    {
        private readonly IServiceProvider _services;
        private readonly IImportProgressService _progressService;
        private Action<decimal> _trackProgressMethod;

        public TrackRepository(GpxDbContext dbContext, IServiceProvider services, IImportProgressService progressService) : base(dbContext)
        {
            _services = services;
            _progressService = progressService;
        }

        public async Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, CancellationToken cancellationToken)
        {
            await using (var dbContext = _services.GetRequiredService<GpxDbContext>())
            {
                await using (var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        var bulkConfig = new BulkConfig()
                        {
                            UnderlyingConnection = GetConnection,
                            UnderlyingTransaction = GetTransaction
                        };

                        await dbContext.BulkInsertAsync(importFilesResult.Tracks, bulkConfig, cancellationToken: cancellationToken);
                        await dbContext.BulkInsertAsync(importFilesResult.TrackSegments, bulkConfig, cancellationToken: cancellationToken);
                        await dbContext.BulkInsertAsync(importFilesResult.TrackPoints, bulkConfig, cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }

                    await transaction.CommitAsync(cancellationToken);
                };
            }
        }

        public async Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, Guid processId, CancellationToken cancellationToken)
        {
            await using (var dbContext = _services.GetRequiredService<GpxDbContext>())
            {
                await using (var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        InitializeImportProgress(processId, importFilesResult);

                        var bulkConfig = new BulkConfig()
                        {
                            UnderlyingConnection = GetConnection,
                            UnderlyingTransaction = GetTransaction
                        };

                        _totalRecordsImported = 0;
                        _trackProgressMethod = GetTracksProgress;
                        await dbContext.BulkInsertAsync(importFilesResult.Tracks, bulkConfig, _trackProgressMethod,
                            cancellationToken: cancellationToken);

                        _totalRecordsImported += importFilesResult.Tracks.Count;
                        _trackProgressMethod = GetTrackSegmentsProgress;
                        await dbContext.BulkInsertAsync(importFilesResult.TrackSegments, bulkConfig,
                            _trackProgressMethod, cancellationToken: cancellationToken);

                        _totalRecordsImported += importFilesResult.TrackSegments.Count;
                        _trackProgressMethod = GetTrackPointsProgress;
                        await dbContext.BulkInsertAsync(importFilesResult.TrackPoints, bulkConfig, _trackProgressMethod,
                            cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                    finally
                    {
                        _progressService.ProgressComplete(processId);
                    }

                    await transaction.CommitAsync(cancellationToken);
                };
            }
        }

        private Guid _processId;
        private int _totalRecordsImported;
        private int _totalRecordsToImport;
        private int _trackCount;
        private int _trackSegmentCount;
        private int _trackPointCount;
        private decimal _percentageComplete;

        private void GetTracksProgress(decimal percentage)
        {
            _percentageComplete = percentage * _trackCount / _totalRecordsToImport;
            _progressService.UpdateProgress(_processId, _percentageComplete);
        }
        private void GetTrackSegmentsProgress(decimal percentage)
        {
            _percentageComplete = (percentage * _trackSegmentCount + _totalRecordsImported) / _totalRecordsToImport;
            _progressService.UpdateProgress(_processId, _percentageComplete);
        }
        private void GetTrackPointsProgress(decimal percentage)
        {
            _percentageComplete = (percentage * _trackPointCount + _totalRecordsImported) / _totalRecordsToImport;
            _progressService.UpdateProgress(_processId, _percentageComplete);
        }

        private void InitializeImportProgress(Guid processId, ImportFilesResult importFilesResult)
        {
            _processId = processId;
            _trackCount = importFilesResult.Tracks.Count;
            _trackSegmentCount = importFilesResult.TrackSegments.Count;
            _trackPointCount = importFilesResult.TrackPoints.Count;
            _totalRecordsToImport = _trackCount + _trackSegmentCount + _trackPointCount;
            _progressService.InitializeProgress(_processId);
        }

        public async Task<Track> GetTrackAsync(Guid trackId)
        {
            var track = await DbContext.Tracks.Where(x => x.Id == trackId).FirstOrDefaultAsync();
            if (track is null)
            {
                //TODO Create default Track
                return default;
            }
            track.TrackSegments = await DbContext.TrackSegments.Where(x => x.TrackId == track.Id).OrderBy(x=>x.Seq).ToListAsync();
            foreach (var segment in track.TrackSegments)
            {
                segment.TrackPoints = await DbContext.TrackPoints.Where(x => x.TrackSegmentId == segment.Id).OrderBy(x=>x.Seq).ToListAsync();
            }

            return track;
        }

        public async Task<IList<string>> GetExistingTracksAsync()
        {
            return await DbContext.Tracks.Select(x => x.Name).ToListAsync();
        }

        public async Task<Track> GetFirstEverActivityAsync()
        {
            return await DbContext.Tracks.OrderBy(x => x.Time).FirstOrDefaultAsync();
        }

        public async Task<Track> GetMostRecentTrackAsync()
        {
            var track = await DbContext.Tracks.OrderByDescending(x => x.Time).FirstOrDefaultAsync();
            if (track is null)
            {
                //TODO Create default Track
                return default;
            }
            track.TrackSegments = await DbContext.TrackSegments.Where(x => x.TrackId == track.Id).OrderBy(x => x.Seq).ToListAsync();
            foreach (var segment in track.TrackSegments)
            {
                segment.TrackPoints = await DbContext.TrackPoints.Where(x => x.TrackSegmentId == segment.Id).OrderBy(x => x.Seq).ToListAsync();
            }

            return track;
        }

        public async Task<(int rank, int count)> GetTrackRankAsync(Track track, double upperBounds, double lowerBounds)
        {
            var tracksInRange = await DbContext.Tracks
                .Where(x => x.Distance >= lowerBounds & x.Distance <= upperBounds & x.ActivityType == track.ActivityType)
                .ToArrayAsync();
            var rank = tracksInRange.Count(x => x.Pace < track.Pace);

            return (rank, tracksInRange.Length);
        }

        public async Task<IList<ActivityListMonth>> GetTrackSummaryByMonthsAsync()
        {
            var result = await DbContext.Tracks
                .GroupBy(x => x.Time.Year*100 + x.Time.Month)
                .Select(g => new ActivityListMonth { ActivityYearMonth = g.Key, Count = g.Count()} )
                .OrderByDescending(g=>g.ActivityYearMonth).ToListAsync();

            return result;
        }

        private static DbConnection GetConnection(DbConnection connection)
        {
            return connection;
        }
        private static DbTransaction GetTransaction(DbTransaction transaction)
        {
            return transaction;
        }
    }
}
