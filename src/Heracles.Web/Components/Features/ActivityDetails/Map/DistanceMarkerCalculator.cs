using Heracles.Application.TrackAggregate;

namespace Heracles.Web.Components.Features.ActivityDetails.Map;

internal static class DistanceMarkerCalculator
{
    private const double EarthRadiusKm = 6371;
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

                var distance = CalculateDistance(previousPoint, point);

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

    private static double CalculateDistance(TrackPoint first, TrackPoint second) {
        var latitudeDifference = DegreesToRadians(second.Latitude - first.Latitude);
        var longitudeDifference = DegreesToRadians(second.Longitude - first.Longitude);

        var a =
            Math.Sin(latitudeDifference / 2) * Math.Sin(latitudeDifference / 2)
            + Math.Cos(DegreesToRadians(first.Latitude))
            * Math.Cos(DegreesToRadians(second.Latitude))
            * Math.Sin(longitudeDifference / 2)
            * Math.Sin(longitudeDifference / 2);

        // Guard against floating-point rounding at the limits.
        a = Math.Clamp(a, 0, 1);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) {
        return degrees * (Math.PI / 180);
    }
}