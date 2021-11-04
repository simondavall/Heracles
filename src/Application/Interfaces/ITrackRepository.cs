using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.GpxTrackAggregate;

namespace Heracles.Application.Interfaces
{

    public interface ITrackRepository : IAsyncRepository<Track>
    {
        Task BulkInsertAsync(IList<Track> tracks, CancellationToken cancellationToken);
        Task<IList<string>> GetExistingTracksAsync();
    }
}
