using System.Data.Common;
using EFCore.BulkExtensions;
using Heracles.Application.Activities;
using Heracles.Application.Data;
using Heracles.Application.Import;
using Heracles.Application.Import.Progress;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Heracles.Infrastructure.Data
{
    public class TrackRepository : ITrackRepository
    {
        private readonly ILogger<TrackRepository> _logger;

        private readonly IDbContextFactory<HeraclesDbContext> _contextFactory;

        public TrackRepository(IDbContextFactory<HeraclesDbContext> contextFactory, ILogger<TrackRepository> logger) {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task SaveImportedFilesAsync(ImportFilesResult importFilesResult,
            TrackImportProgress trackProgress,
            CancellationToken cancellationToken) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try {
                var bulkConfig = new BulkConfig { UnderlyingConnection = GetConnection, UnderlyingTransaction = GetTransaction };

                cancellationToken.ThrowIfCancellationRequested();

                trackProgress.SetTrackingProgressMethod(TrackImportMethod.TrackImport);
                await dbContext.BulkInsertAsync(
                    importFilesResult.Tracks,
                    bulkConfig,
                    trackProgress.TrackProgressMethod,
                    cancellationToken: cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                trackProgress.SetTrackingProgressMethod(TrackImportMethod.SegmentImport);
                await dbContext.BulkInsertAsync(
                    importFilesResult.TrackSegments,
                    bulkConfig,
                    trackProgress.TrackProgressMethod,
                    cancellationToken: cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                trackProgress.SetTrackingProgressMethod(TrackImportMethod.PointsImport);
                await dbContext.BulkInsertAsync(
                    importFilesResult.TrackPoints,
                    bulkConfig,
                    trackProgress.TrackProgressMethod,
                    cancellationToken: cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                await transaction.CommitAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
            catch (Exception exception) {
                await transaction.RollbackAsync(CancellationToken.None);
                _logger.LogError(exception, "Failed to persist imported GPX files");
                throw;
            }
        }

        public async Task<bool> DeleteTrackAsync(Guid trackId) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var deleteSucceeded = false;
            var track = await GetTrackAsync(trackId);
            if (track != null) {
                var changes = dbContext.Tracks.Remove(track);
                if (changes.State == EntityState.Deleted) {
                    await dbContext.SaveChangesAsync();
                    deleteSucceeded = true;
                }
            }

            return deleteSucceeded;
        }

        public async Task<Track?> GetTrackAsync(Guid trackId) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == trackId);
            if (track is null) {
                return default;
            }

            track.TrackSegments = await dbContext.TrackSegments.Where(x => x.TrackId == track.Id).OrderBy(x => x.Seq).ToListAsync();
            foreach (var segment in track.TrackSegments) {
                segment.TrackPoints = await dbContext.TrackPoints.Where(x => x.TrackSegmentId == segment.Id).OrderBy(x => x.Seq).ToListAsync();
            }

            return track;
        }

        public async Task<IList<string>> GetExistingTracksAsync() {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            return await dbContext.Tracks.Select(x => x.Name).ToListAsync();
        }

        public async Task<Track?> GetFirstEverActivityAsync() {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            return await dbContext.Tracks.OrderBy(x => x.Time).FirstOrDefaultAsync();
        }

        public async Task<Track?> GetMostRecentTrackAsync() {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var track = await dbContext.Tracks.OrderByDescending(x => x.Time).FirstOrDefaultAsync();
            if (track is null) {
                return default;
            }

            track.TrackSegments = await dbContext.TrackSegments.Where(x => x.TrackId == track.Id).OrderBy(x => x.Seq).ToListAsync();
            foreach (var segment in track.TrackSegments) {
                segment.TrackPoints = await dbContext.TrackPoints.Where(x => x.TrackSegmentId == segment.Id).OrderBy(x => x.Seq).ToListAsync();
            }

            return track;
        }

        public async Task<Track[]> GetTracksInRangeAsync(double upperBounds, double lowerBounds, ActivityType activityType) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            return await dbContext
                .Tracks
                .Where(x => x.Distance >= lowerBounds & x.Distance <= upperBounds & x.ActivityType == activityType)
                .ToArrayAsync();
        }

        public async Task<IList<Track>> GetTracksByDateRangeAsync(DateTime startDate, DateTime endDate) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var result = await dbContext
                .Tracks
                .Where(t => t.Time > startDate & t.Time < endDate)
                .OrderByDescending(t => t.Time)
                .ToListAsync();

            return result;
        }

        public async Task<IList<ActivityListMonth>> GetTrackSummaryByMonthsAsync() {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var result = await dbContext
                .Tracks
                .GroupBy(x => x.Time.Year * 100 + x.Time.Month)
                .Select(g => new ActivityListMonth { ActivityYearMonth = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.ActivityYearMonth)
                .ToListAsync();

            return result;
        }

        public async Task<IList<ActivityListYear>> GetTrackSummaryByYearAsync() {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var result = await dbContext
                .Tracks
                .GroupBy(x => x.Time.Year)
                .Select(g => new ActivityListYear { ActivityYear = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.ActivityYear)
                .ToListAsync();

            return result;
        }

        private static DbConnection GetConnection(DbConnection connection) {
            return connection;
        }

        private static DbTransaction GetTransaction(DbTransaction transaction) {
            return transaction;
        }
    }
}