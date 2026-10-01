using Heracles.Application.Activities;
using Heracles.Application.Import;
using Heracles.Application.Import.Progress;

namespace Heracles.Application.Data
{
    public interface ITrackRepository
    {
        Task<bool> DeleteTrackAsync(Guid trackId);
        Task<Track?> GetTrackAsync(Guid trackId);
        Task<IList<string>> GetExistingTracksAsync();
        Task<Track?> GetFirstEverActivityAsync();
        Task<Track?> GetMostRecentTrackAsync(ActivityType? activityType = null);
        Task<Track[]> GetTracksInRangeAsync(double upperBounds, double lowerBounds, ActivityType activityType);
        Task<IList<Track>> GetTracksByDateRangeAsync(DateTime startDate, DateTime endDate, ActivityType? activityType = null);
        Task<IList<ActivityListMonth>> GetTrackSummaryByMonthsAsync(ActivityType? activityType = null);
        Task<IList<ActivityListYear>> GetTrackSummaryByYearAsync(ActivityType? activityType = null);
        Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, TrackImportProgress trackProgress, CancellationToken cancellationToken);
    }
}
