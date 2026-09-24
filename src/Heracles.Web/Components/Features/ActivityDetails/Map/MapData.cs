namespace Heracles.Web.Components.Features.ActivityDetails.Map;

public sealed record ActivityMapData(
    IReadOnlyList<ActivityMapSegment> Segments,
    IReadOnlyList<ActivityMapDistanceMarker> DistanceMarkers);

public sealed record ActivityMapSegment(
    IReadOnlyList<ActivityMapCoordinate> Coordinates);

public sealed record ActivityMapCoordinate(
    double Longitude,
    double Latitude);

public sealed record ActivityMapDistanceMarker(
    int Distance,
    double Longitude,
    double Latitude);