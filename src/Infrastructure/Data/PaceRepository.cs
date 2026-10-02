using Heracles.Application.Pace;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Infrastructure.Data;

public sealed class PaceRepository : IPaceRepository
{
    private readonly IDbContextFactory<HeraclesDbContext> _contextFactory;

    public PaceRepository(
        IDbContextFactory<HeraclesDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<PaceData>> GetAsync(
        Guid trackId,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(
                cancellationToken);

        return await context.PaceData
            .AsNoTracking()
            .Where(x => x.TrackId == trackId)
            .OrderBy(x => x.Seq)
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceAsync(
        Guid trackId,
        IReadOnlyCollection<PaceData> paceData,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(
                cancellationToken);

        await using var transaction =
            await context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            await context.PaceData
                .Where(x => x.TrackId == trackId)
                .ExecuteDeleteAsync(cancellationToken);

            context.PaceData.AddRange(paceData);

            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }
}