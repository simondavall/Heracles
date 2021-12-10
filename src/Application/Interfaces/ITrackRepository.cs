using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.Entities;
using Heracles.Application.Services.Import;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Interfaces
{

    public interface ITrackRepository : IAsyncRepository<Track, Guid>
    {
        Task<Track> GetTrackAsync(Guid trackId);
        Task<IList<string>> GetExistingTracksAsync();
        Task<Track> GetMostRecentTrack();
        Task<(int rank, int count)> GetTrackRank(Track track, double upperBounds, double lowerBounds);
        Task<IList<ActivityListMonth>> GetTrackSummaryByMonths();
        Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, CancellationToken cancellationToken);
    }
}
