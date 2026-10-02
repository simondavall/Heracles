using Heracles.Application.TrackPoints;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Infrastructure.Data;

public sealed class TrackPointDataRepository : ITrackPointDataRepository
{
    private readonly IDbContextFactory<HeraclesDbContext> _contextFactory;

    public TrackPointDataRepository(
        IDbContextFactory<HeraclesDbContext> contextFactory) {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<TrackPointData>> GetAsync(
        Guid trackId,
        CancellationToken cancellationToken = default) {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.TrackPointData
            .AsNoTracking()
            .Where(x => x.TrackId == trackId)
            .OrderBy(x => x.Seq)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveAsync(
        IReadOnlyCollection<TrackPointData> trackPointData,
        CancellationToken cancellationToken = default) {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        context.TrackPointData.AddRange(trackPointData);

        await context.SaveChangesAsync(cancellationToken);
    }
}