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
        Task<IList<string>> GetExistingTracksAsync();
        Task<IList<ActivityListMonth>> GetTrackSummaryByMonths();
        Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, CancellationToken cancellationToken);
    }
}
