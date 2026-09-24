namespace Heracles.Web.Components.Features.ActivityDetails.Map;

public sealed record ActivityMapData(
    IReadOnlyList<ActivityMapSegment> Segments);

public sealed record ActivityMapSegment(
    IReadOnlyList<ActivityMapCoordinate> Coordinates);

public sealed record ActivityMapCoordinate(
    double Longitude,
    double Latitude);