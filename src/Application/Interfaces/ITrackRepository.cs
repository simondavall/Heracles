using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.Services.Import;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Interfaces
{

    public interface ITrackRepository : IAsyncRepository<Track, Guid>
    {
        Task<IList<string>> GetExistingTracksAsync();
        Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, CancellationToken cancellationToken);
    }
}
