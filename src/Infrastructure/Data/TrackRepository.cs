using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Heracles.Application.GpxTrackAggregate;
using Heracles.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Infrastructure.Data
{
    public class TrackRepository : EfRepository<Track>, ITrackRepository
    {
        public TrackRepository(GpxDbContext dbContext) : base(dbContext)
        {}

        public async Task BulkInsertAsync(IList<Track> tracks, CancellationToken cancellationToken)
        {
            BulkConfig bulkConfig = new()
            {
                IncludeGraph = true
            };

            await DbContext.BulkInsertAsync(tracks, bulkConfig, cancellationToken: cancellationToken);
        }

        public async Task<IList<string>> GetExistingTracksAsync()
        {
            return await DbContext.Tracks.Select(x => x.Name).ToListAsync();
        }
    }
}
