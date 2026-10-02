using Heracles.Application.Data;

namespace Heracles.Web.Components.Features.ActivityDetails.Map;

internal static class DistanceMarkerCalculator
{
    private const double MarkerIntervalKm = 1;

    public static IReadOnlyList<MapDistanceMarker> Calculate(Track track) {
        var markers = new List<MapDistanceMarker>();

        var cumulativeDistance = 0d;
        var nextMarkerDistance = MarkerIntervalKm;

        var segments = track.TrackSegments
            .OrderBy(segment => segment.Seq);

        foreach (var segment in segments) {
            TrackPoint? previousPoint = null;

            foreach (var point in segment.TrackPoints.OrderBy(point => point.Seq)) {
                if (previousPoint is null) {
                    previousPoint = point;
                    continue;
                }
                
                var distance = TrackPointDistanceCalculator.Calculate(previousPoint, point);

                if (distance <= 0) {
                    previousPoint = point;
                    continue;
                }

                var previousDistance = cumulativeDistance;

                cumulativeDistance += distance;

                while (nextMarkerDistance <= cumulativeDistance) {
                    var fraction = (nextMarkerDistance - previousDistance) / distance;

                    var longitude = previousPoint.Longitude + (point.Longitude - previousPoint.Longitude) * fraction;

                    var latitude = previousPoint.Latitude + (point.Latitude - previousPoint.Latitude) * fraction;

                    markers.Add(
                        new MapDistanceMarker(
                            (int)nextMarkerDistance,
                            longitude,
                            latitude));

                    nextMarkerDistance += MarkerIntervalKm;
                }

                previousPoint = point;
            }
        }

        return markers;
    }
}