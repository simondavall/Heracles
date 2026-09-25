using System.Globalization;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.ActivityDetails;

public partial class MetricsBar
{
    [Inject]
    private IActivityService ActivityService { get; set; } = null!;

    [Parameter]
    public Track Track { get; set; } = null!;

    private string Distance => Track.Distance.ToString("0.00", CultureInfo.InvariantCulture);

    private string Duration => ToFormattedString(Track.Duration);

    private string Pace => ToFormattedString(Track.Pace);

    private string Rank { get; set; } = string.Empty;

    private string RankCount { get; set; } = string.Empty;

    protected override async Task OnParametersSetAsync() {
        var (rank, count) = await ActivityService.GetActivityRankAsync(Track);

        Rank = rank.ToString(CultureInfo.InvariantCulture);
        RankCount = $"/{count.ToString(CultureInfo.InvariantCulture)}";
    }

    private static string ToFormattedString(TimeSpan span) {
        return span.TotalHours >= 1
            ? $"{span.TotalHours:#0}:{span.Minutes:00}:{span.Seconds:00}"
            : $"{span.Minutes:#0}:{span.Seconds:00}";
    }
}