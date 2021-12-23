using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.Entities;
using Heracles.Application.Services.Import;
using Heracles.Application.Services.Import.Progress;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Interfaces
{
    public interface ITrackRepository : IAsyncRepository<Track, Guid>
    {
        Task<bool> DeleteTrackAsync(Guid trackId);
        Task<Track> GetTrackAsync(Guid trackId);
        Task<IList<string>> GetExistingTracksAsync();
        Task<Track> GetFirstEverActivityAsync();
        Task<Track> GetMostRecentTrackAsync();
        Task<(int rank, int count)> GetTrackRankAsync(Track track, double upperBounds, double lowerBounds);
        Task<IList<ActivityListMonth>> GetTrackSummaryByMonthsAsync();
        Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, TrackImportProgress trackProgress, CancellationToken cancellationToken);
    }
}
