namespace Heracles.Application.Tracks;

public static class TrackPointDistanceCalculator
{
    private const double EarthRadiusKm = 6371;

    public static double Calculate(TrackPoint first, TrackPoint second)
    {
        var latitudeDifference = DegreesToRadians(second.Latitude - first.Latitude);
        var longitudeDifference = DegreesToRadians(second.Longitude - first.Longitude);

        var a =
            Math.Sin(latitudeDifference / 2)
            * Math.Sin(latitudeDifference / 2)
            + Math.Cos(DegreesToRadians(first.Latitude))
            * Math.Cos(DegreesToRadians(second.Latitude))
            * Math.Sin(longitudeDifference / 2)
            * Math.Sin(longitudeDifference / 2);

        a = Math.Clamp(a, 0, 1);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}