using Heracles.Application.Tracks;

namespace Heracles.Application.Activities.Splits;

public interface ISplitService
{
    Task<IReadOnlyList<Split>> GetSplitsAsync(Track track, CancellationToken cancellationToken = default);
}

public sealed class SplitService : ISplitService
{
    private const double SplitDistance = 1d;

    private readonly ITrackPointDataService _trackPointDataService;

    public SplitService(ITrackPointDataService trackPointDataService) {
        _trackPointDataService = trackPointDataService;
    }

    public async Task<IReadOnlyList<Split>> GetSplitsAsync(Track track, CancellationToken cancellationToken = default) {
        var trackPointData = await _trackPointDataService.GetAsync(track, cancellationToken);

        if (trackPointData.Count < 2)
            return [];

        return CalculateSplits(trackPointData);
    }

    private static IReadOnlyList<Split> CalculateSplits(IReadOnlyList<TrackPointData> trackPointData) {
        var totalDistance = trackPointData[^1].CumulativeDistance;

        if (totalDistance <= 0)
            return [];

        var splits = new List<Split>();
        var start = GetBoundary(trackPointData, 0);
        var boundaryDistance = SplitDistance;

        while (boundaryDistance < totalDistance) {
            var end = GetBoundary(trackPointData, boundaryDistance);

            AddSplit(splits, start, end);

            start = end;
            boundaryDistance += SplitDistance;
        }

        var finish = GetBoundary(trackPointData, totalDistance);

        if (finish.Distance > start.Distance)
            AddSplit(splits, start, finish);

        return splits;
    }

    private static void AddSplit(ICollection<Split> splits, SplitBoundary start, SplitBoundary end) {
        var distance = end.Distance - start.Distance;
        var elapsedSeconds = end.CumulativeTime - start.CumulativeTime;

        if (distance <= 0 || elapsedSeconds <= 0)
            return;

        splits.Add(
            new Split(
                end.Distance,
                elapsedSeconds / distance,
                end.Elevation - start.Elevation));
    }

    private static SplitBoundary GetBoundary(IReadOnlyList<TrackPointData> trackPointData, double distance) {
        if (distance <= trackPointData[0].CumulativeDistance)
            return FromTrackPointData(trackPointData[0]);

        for (var index = 1; index < trackPointData.Count; index++) {
            var end = trackPointData[index];

            if (end.CumulativeDistance < distance)
                continue;

            var start = trackPointData[index - 1];
            var intervalDistance = end.CumulativeDistance - start.CumulativeDistance;

            if (intervalDistance <= 0)
                return FromTrackPointData(end);

            var fraction = (distance - start.CumulativeDistance) / intervalDistance;

            return new SplitBoundary(
                distance,
                Interpolate(start.CumulativeTime, end.CumulativeTime, fraction),
                Interpolate(start.Elevation, end.Elevation, fraction));
        }

        return FromTrackPointData(trackPointData[^1]);
    }

    private static SplitBoundary FromTrackPointData(TrackPointData trackPointData) {
        return new SplitBoundary(
            trackPointData.CumulativeDistance,
            trackPointData.CumulativeTime,
            trackPointData.Elevation);
    }

    private static double Interpolate(double start, double end, double fraction) {
        return start + ((end - start) * fraction);
    }

    private sealed record SplitBoundary(
        double Distance,
        double CumulativeTime,
        double Elevation);
}

public sealed record Split(
    double Distance,
    double SecondsPerKilometre,
    double ElevationDifference);