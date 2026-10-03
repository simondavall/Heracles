namespace Heracles.Web.Components.Features.ActivityDetails.Speed;

internal sealed record SpeedChartData(
    IReadOnlyList<SpeedChartPoint> Points);

internal sealed record SpeedChartPoint(
    int TrackPointId,
    double Distance,
    double Speed);