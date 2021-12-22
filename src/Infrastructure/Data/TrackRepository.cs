using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Heracles.Application.Entities;
using Heracles.Application.Enums;
using Heracles.Application.Interfaces;
using Heracles.Application.Services.Import;
using Heracles.Application.Services.Import.Progress;
using Heracles.Application.TrackAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Infrastructure.Data
{
    public class TrackRepository : EfRepository<Track, Guid>, ITrackRepository
    {
        private readonly IServiceProvider _services;
        private readonly IImportProgressService _progressService;

        public TrackRepository(GpxDbContext dbContext, IServiceProvider services, IImportProgressService progressService) : base(dbContext)
        {
            _services = services;
            _progressService = progressService;
        }

        public async Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, TrackImportProgress trackProgress, CancellationToken cancellationToken)
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

                        trackProgress.SetTrackingProgressMethod(TrackImportMethod.TrackImport);
                        await dbContext.BulkInsertAsync(importFilesResult.Tracks, bulkConfig, trackProgress.TrackProgressMethod, cancellationToken: cancellationToken);

                        trackProgress.SetTrackingProgressMethod(TrackImportMethod.SegmentImport);
                        await dbContext.BulkInsertAsync(importFilesResult.TrackSegments, bulkConfig, trackProgress.TrackProgressMethod, cancellationToken: cancellationToken);

                        trackProgress.SetTrackingProgressMethod(TrackImportMethod.PointsImport);
                        await dbContext.BulkInsertAsync(importFilesResult.TrackPoints, bulkConfig, trackProgress.TrackProgressMethod, cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                    finally
                    {
                        _progressService.ProgressComplete(trackProgress.ProcessId);
                    }

                    await transaction.CommitAsync(cancellationToken);
                };
            }
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
