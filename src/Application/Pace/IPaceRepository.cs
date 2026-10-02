namespace Heracles.Application.Pace;

public interface IPaceRepository
{
    Task<IReadOnlyList<PaceData>> GetAsync(Guid trackId, CancellationToken cancellationToken = default);
    Task ReplaceAsync(Guid trackId, IReadOnlyCollection<PaceData> paceData, CancellationToken cancellationToken = default);
}