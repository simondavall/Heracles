namespace Heracles.Web.Components.Features.ActivityDetails.Map;

public sealed record MapData(
    IReadOnlyList<MapSegment> Segments,
    IReadOnlyList<MapDistanceMarker> DistanceMarkers);

public sealed record MapSegment(
    IReadOnlyList<MapCoordinate> Coordinates);

public sealed record MapCoordinate(
    double Longitude,
    double Latitude);

public sealed record MapDistanceMarker(
    int Distance,
    double Longitude,
    double Latitude);