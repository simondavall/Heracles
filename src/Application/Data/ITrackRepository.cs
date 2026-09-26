using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.Activities;
using Heracles.Application.Entities;
using Heracles.Application.Import;
using Heracles.Application.Import.Progress;

namespace Heracles.Application.Data
{
    public interface ITrackRepository
    {
        Task<bool> DeleteTrackAsync(Guid trackId);
        Task<Track> GetTrackAsync(Guid trackId);
        Task<IList<string>> GetExistingTracksAsync();
        Task<Track> GetFirstEverActivityAsync();
        Task<Track> GetMostRecentTrackAsync();
        Task<Track[]> GetTracksInRangeAsync(double upperBounds, double lowerBounds, ActivityType activityType);
        Task<IList<Track>> GetTracksByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IList<ActivityListMonth>> GetTrackSummaryByMonthsAsync();
        Task<IList<ActivityListYear>> GetTrackSummaryByYearAsync();
        Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, TrackImportProgress trackProgress, CancellationToken cancellationToken);
    }
}
