using Heracles.Application.Configuration;
using Heracles.Application.Data;

namespace Heracles.Application.Pace;

public interface IPaceService
{
    Task<IReadOnlyList<PaceObservation>> GetPaceAsync(Track track, CancellationToken cancellationToken = default);
}

public sealed class PaceService : IPaceService
{
    private readonly IPaceRepository _paceRepository;
    private readonly PaceSettings _settings;

    public PaceService(IPaceRepository paceRepository, PaceSettings settings) {
        _paceRepository = paceRepository;
        _settings = settings;
    }

    public async Task<IReadOnlyList<PaceObservation>> GetPaceAsync(Track track, CancellationToken cancellationToken = default) {
        var orderedPoints = GetOrderedPoints(track);

        if (orderedPoints.Count < 2)
            return [];

        var paceData = await _paceRepository.GetAsync(track.Id, cancellationToken);
        if (!IsComplete(paceData, orderedPoints)) {
            paceData = CreatePaceData(track.Id, orderedPoints);
            await _paceRepository.ReplaceAsync(track.Id, paceData, cancellationToken);
        }

        return CalculateObservations(paceData);
    }

    private static IReadOnlyList<OrderedTrackPoint> GetOrderedPoints(Track track) {
        var points = new List<OrderedTrackPoint>();

        foreach (var segment in track.TrackSegments.OrderBy(x => x.Seq)) {
            var firstPointInSegment = true;

            foreach (var point in segment.TrackPoints.OrderBy(x => x.Seq)) {
                points.Add(new OrderedTrackPoint(point, firstPointInSegment));
                firstPointInSegment = false;
            }
        }

        return points;
    }

    private static bool IsComplete(IReadOnlyList<PaceData> paceData, IReadOnlyList<OrderedTrackPoint> points) {
        if (paceData.Count != points.Count)
            return false;

        for (var index = 0; index < points.Count; index++) {
            var data = paceData[index];

            if (data.Seq != index)
                return false;

            if (data.TrackPointId != points[index].Point.Id)
                return false;
        }

        return true;
    }

    private static IReadOnlyList<PaceData> CreatePaceData(Guid trackId, IReadOnlyList<OrderedTrackPoint> points) {
        var result = new List<PaceData>(points.Count);

        var cumulativeDistance = 0d;
        var cumulativeTime = 0;

        TrackPoint? previousPoint = null;

        for (var index = 0; index < points.Count; index++) {
            var orderedPoint = points[index];
            var point = orderedPoint.Point;

            if (!orderedPoint.FirstPointInSegment && previousPoint is not null) {
                cumulativeDistance += TrackPointDistanceCalculator.Calculate(previousPoint, point);

                var elapsedSeconds = (int)(point.Time - previousPoint.Time).TotalSeconds;
                if (elapsedSeconds > 0)
                    cumulativeTime += elapsedSeconds;
            }

            result.Add(
                new PaceData {
                    TrackId = trackId,
                    TrackPointId = point.Id,
                    Seq = index,
                    CumulativeDistance = cumulativeDistance,
                    CumulativeTime = cumulativeTime
                });

            previousPoint = point;
        }

        return result;
    }

    private IReadOnlyList<PaceObservation> CalculateObservations(IReadOnlyList<PaceData> paceData) {
        var observations = new List<PaceObservation>();

        for (var centre = 0; centre < paceData.Count; centre += _settings.Stride)
            AddObservation(paceData, centre, observations);

        var lastIndex = paceData.Count - 1;

        if (lastIndex % _settings.Stride != 0)
            AddObservation(paceData, lastIndex, observations);

        return observations;
    }

    private void AddObservation(IReadOnlyList<PaceData> paceData, int centre, ICollection<PaceObservation> observations) {
        var start = Math.Max(0, centre - _settings.WindowRadius);
        var end = Math.Min(paceData.Count - 1, centre + _settings.WindowRadius);

        var distance = paceData[end].CumulativeDistance - paceData[start].CumulativeDistance;
        var elapsedSeconds = paceData[end].CumulativeTime - paceData[start].CumulativeTime;

        if (distance <= 0 || elapsedSeconds <= 0)
            return;

        observations.Add(new PaceObservation(paceData[centre].CumulativeDistance, elapsedSeconds / distance));
    }

    private sealed record OrderedTrackPoint(
        TrackPoint Point,
        bool FirstPointInSegment);
}

public sealed record PaceObservation(
    double Distance,
    double SecondsPerKilometre);