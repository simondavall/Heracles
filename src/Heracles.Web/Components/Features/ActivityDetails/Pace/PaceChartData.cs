namespace Heracles.Web.Components.Features.ActivityDetails.Pace;

internal sealed record PaceChartData(IReadOnlyList<PaceChartPoint> Points);

internal sealed record PaceChartPoint(
    int TrackPointId,
    double Distance,
    double Pace);