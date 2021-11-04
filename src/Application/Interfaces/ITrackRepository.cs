using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Interfaces
{

    public interface ITrackRepository : IAsyncRepository<Track, Guid>
    {
        Task BulkInsertAsync(IList<Track> tracks, CancellationToken cancellationToken);
        Task BulkInsertAsync(IList<TrackSegment> trackSegments, CancellationToken cancellationToken);
        Task BulkInsertAsync(IList<TrackPoint> trackSegments, CancellationToken cancellationToken);
        Task<IList<string>> GetExistingTracksAsync();
    }
}
