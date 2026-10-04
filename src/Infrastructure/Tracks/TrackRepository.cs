using System.Data.Common;
using EFCore.BulkExtensions;
using Heracles.Application.Activities;
using Heracles.Application.Import;
using Heracles.Application.Import.Progress;
using Heracles.Application.Tracks;
using Heracles.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Heracles.Infrastructure.Tracks
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
            ImportProgress progress,
            CancellationToken cancellationToken) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try {
                var bulkConfig = new BulkConfig { UnderlyingConnection = GetConnection, UnderlyingTransaction = GetTransaction };

                cancellationToken.ThrowIfCancellationRequested();

                progress.SetTrackingProgressMethod(ImportPhase.TrackImport);
                await dbContext.BulkInsertAsync(
                    importFilesResult.Tracks,
                    bulkConfig,
                    progress.TrackProgressMethod,
                    cancellationToken: cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                progress.SetTrackingProgressMethod(ImportPhase.SegmentImport);
                await dbContext.BulkInsertAsync(
                    importFilesResult.TrackSegments,
                    bulkConfig,
                    progress.TrackProgressMethod,
                    cancellationToken: cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                progress.SetTrackingProgressMethod(ImportPhase.PointsImport);
                await dbContext.BulkInsertAsync(
                    importFilesResult.TrackPoints,
                    bulkConfig,
                    progress.TrackProgressMethod,
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

        public async Task<IList<ActivityType>> GetActivityTypesAsync()
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            return await dbContext.Tracks
                .Select(x => x.ActivityType)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
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

        public async Task<Track?> GetMostRecentTrackAsync(ActivityType? activityType = null) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var query = dbContext.Tracks.AsQueryable();

            if (activityType.HasValue)
                query = query.Where(x => x.ActivityType == activityType.Value);

            var track = await query
                .OrderByDescending(x => x.Time)
                .FirstOrDefaultAsync();

            if (track is null)
                return default;

            track.TrackSegments = await dbContext.TrackSegments
                .Where(x => x.TrackId == track.Id)
                .OrderBy(x => x.Seq)
                .ToListAsync();

            foreach (var segment in track.TrackSegments) {
                segment.TrackPoints = await dbContext.TrackPoints
                    .Where(x => x.TrackSegmentId == segment.Id)
                    .OrderBy(x => x.Seq)
                    .ToListAsync();
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

        public async Task<IList<Track>> GetTracksByDateRangeAsync(DateTime startDate, DateTime endDate, ActivityType? activityType = null) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var query = dbContext.Tracks
                .Where(t => t.Time > startDate & t.Time < endDate);

            if (activityType.HasValue)
                query = query.Where(t => t.ActivityType == activityType.Value);

            return await query
                .OrderByDescending(t => t.Time)
                .ToListAsync();
        }

        public async Task<IList<ActivityListMonth>> GetTrackSummaryByMonthsAsync(ActivityType? activityType = null) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var query = dbContext.Tracks.AsQueryable();

            if (activityType.HasValue)
                query = query.Where(x => x.ActivityType == activityType.Value);

            return await query
                .GroupBy(x => x.Time.Year * 100 + x.Time.Month)
                .Select(g => new ActivityListMonth { ActivityYearMonth = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.ActivityYearMonth)
                .ToListAsync();
        }

        public async Task<IList<ActivityListYear>> GetTrackSummaryByYearAsync(ActivityType? activityType = null) {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var query = dbContext.Tracks.AsQueryable();

            if (activityType.HasValue)
                query = query.Where(x => x.ActivityType == activityType.Value);

            return await query
                .GroupBy(x => x.Time.Year)
                .Select(g => new ActivityListYear { ActivityYear = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.ActivityYear)
                .ToListAsync();
        }

        private static DbConnection GetConnection(DbConnection connection) {
            return connection;
        }

        private static DbTransaction GetTransaction(DbTransaction transaction) {
            return transaction;
        }
    }
}