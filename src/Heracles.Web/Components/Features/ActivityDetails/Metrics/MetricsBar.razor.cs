using System.Globalization;
using Heracles.Application.Activities;
using Heracles.Application.Tracks;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.ActivityDetails.Metrics;

public partial class MetricsBar
{
    [Inject]
    private IActivityService ActivityService { get; set; } = null!;

    [Parameter]
    public Track Track { get; set; } = null!;

    private string Distance => Track.Distance.ToString("0.00", CultureInfo.InvariantCulture);

    private string Duration => ToFormattedString(Track.Duration);

    private string Pace => ToFormattedString(Track.Pace);

    private bool _showSplits;
    private bool _showRanking;

    private ActivityRank? _activityRank;

    private string Rank { get; set; } = string.Empty;
    private string RankCount { get; set; } = string.Empty;

    protected override async Task OnParametersSetAsync() {
        _showSplits = false;
        _showRanking = false;

        _activityRank = await ActivityService.GetActivityRankAsync(Track);

        Rank = _activityRank.Rank.ToString(CultureInfo.InvariantCulture);
        RankCount = $"/{_activityRank.Count.ToString(CultureInfo.InvariantCulture)}";
    }

    private void ShowSplits() {
        _showSplits = true;
    }

    private void CloseSplits() {
        _showSplits = false;
    }

    private void ShowRanking() {
        _showRanking = true;
    }

    private void CloseRanking() {
        _showRanking = false;
    }

    private static string ToFormattedString(TimeSpan span) {
        return span.TotalHours >= 1
            ? $"{span.TotalHours:#0}:{span.Minutes:00}:{span.Seconds:00}"
            : $"{span.Minutes:#0}:{span.Seconds:00}";
    }
}