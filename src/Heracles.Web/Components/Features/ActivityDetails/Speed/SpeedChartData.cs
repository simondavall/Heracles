namespace Heracles.Web.Components.Features.ActivityDetails.Speed;

internal sealed record SpeedChartData(
    IReadOnlyList<SpeedChartPoint> Points);

internal sealed record SpeedChartPoint(
    double Distance,
    double Speed);