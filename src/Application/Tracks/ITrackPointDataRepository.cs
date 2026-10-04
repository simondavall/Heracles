namespace Heracles.Application.Tracks;

public interface ITrackPointDataRepository
{
    Task<IReadOnlyList<TrackPointData>> GetAsync(Guid trackId, CancellationToken cancellationToken = default);
    Task SaveAsync(IReadOnlyCollection<TrackPointData> trackPointData, CancellationToken cancellationToken = default);
}