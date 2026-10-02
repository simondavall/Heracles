namespace Heracles.Web.Components.Features.ActivityDetails.Elevation;

internal sealed record ElevationChartData(
    IReadOnlyList<ElevationChartPoint> Points,
    double? MinimumElevation,
    double? MaximumElevation);

internal sealed record ElevationChartPoint(
    double Distance,
    double Elevation);