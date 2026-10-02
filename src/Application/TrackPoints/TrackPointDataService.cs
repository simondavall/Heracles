using Heracles.Application.Data;

namespace Heracles.Application.TrackPoints;

public interface ITrackPointDataService
{
    Task<IReadOnlyList<TrackPointData>> GetAsync(Track track, CancellationToken cancellationToken = default);
}

public sealed class TrackPointDataService : ITrackPointDataService
{
    private readonly ITrackPointDataRepository _repository;
    private readonly SemaphoreSlim _generationLock = new(1, 1);

    public TrackPointDataService(ITrackPointDataRepository repository) {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TrackPointData>> GetAsync(Track track, CancellationToken cancellationToken = default) {
        var trackPointData = await _repository.GetAsync(track.Id, cancellationToken);

        if (trackPointData.Count > 0)
            return trackPointData;

        await _generationLock.WaitAsync(cancellationToken);

        try {
            trackPointData = await _repository.GetAsync(track.Id, cancellationToken);

            if (trackPointData.Count > 0)
                return trackPointData;

            trackPointData = CreateTrackPointData(track);

            if (trackPointData.Count > 0)
                await _repository.SaveAsync(trackPointData, cancellationToken);

            return trackPointData;
        }
        finally {
            _generationLock.Release();
        }
    }

    private static IReadOnlyList<TrackPointData> CreateTrackPointData(Track track) {
        var result = new List<TrackPointData>();

        var cumulativeDistance = 0d;
        var cumulativeTime = 0;

        TrackPoint? previousPoint = null;
        var sequence = 0;

        foreach (var segment in track.TrackSegments.OrderBy(x => x.Seq)) {
            var firstPointInSegment = true;

            foreach (var point in segment.TrackPoints.OrderBy(x => x.Seq)) {
                if (!firstPointInSegment && previousPoint is not null) {
                    cumulativeDistance += TrackPointDistanceCalculator.Calculate(previousPoint, point);

                    var elapsedSeconds = (int)(point.Time - previousPoint.Time).TotalSeconds;
                    if (elapsedSeconds > 0) 
                        cumulativeTime += elapsedSeconds;
                }

                result.Add(
                    new TrackPointData {
                        TrackId = track.Id,
                        TrackPointId = point.Id,
                        Seq = sequence,
                        CumulativeDistance = cumulativeDistance,
                        CumulativeTime = cumulativeTime,
                        Elevation = point.Elevation
                    });

                previousPoint = point;
                firstPointInSegment = false;
                sequence++;
            }
        }

        return result;
    }
}