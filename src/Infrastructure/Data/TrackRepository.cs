using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Infrastructure.Data
{
    public class TrackRepository : EfRepository<Track, Guid>, ITrackRepository
    {
        public TrackRepository(GpxDbContext dbContext) : base(dbContext)
        {}

        public async Task BulkInsertAsync(IList<Track> tracks, CancellationToken cancellationToken)
        {
            await DbContext.BulkInsertAsync(tracks, cancellationToken: cancellationToken);
        }

        public async Task BulkInsertAsync(IList<TrackSegment> trackSegments, CancellationToken cancellationToken)
        {
            await DbContext.BulkInsertAsync(trackSegments, cancellationToken: cancellationToken);
        }

        public async Task BulkInsertAsync(IList<TrackPoint> trackPoints, CancellationToken cancellationToken)
        {
            await DbContext.BulkInsertAsync(trackPoints, cancellationToken: cancellationToken);
        }

        public async Task<IList<string>> GetExistingTracksAsync()
        {
            return await DbContext.Tracks.Select(x => x.Name).ToListAsync();
        }
    }
}
