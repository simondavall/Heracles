using System.Globalization;
using Heracles.Application.Extensions;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.Activity;

public partial class StatsBar : ComponentBase
{
    [Inject]
    private IActivityService ActivityService { get; set; } = default!;

    [Parameter]
    public required Track Track { get; set; }

    private string? DistanceTitle { get; set; }
    private string? DistanceValue { get; set; }
    private string? DurationTitle { get; set; }
    private string? DurationValue { get; set; }
    private string? PaceTitle { get; set; }
    private string? PaceValue { get; set; }
    private string? RankTitle { get; set; }
    private string? RankValue { get; set; }
    private string? RankCount { get; set; }
    private string? CaloriesTitle { get; set; }
    private string? CaloriesValue { get; set; }

    protected override async Task OnParametersSetAsync() {
        var (rank, count) = await ActivityService.GetActivityRankAsync(Track);

        DistanceTitle = "km";
        DistanceValue = Track.Distance.ToString("0.00", CultureInfo.InvariantCulture);

        DurationTitle = "Duration";
        DurationValue = Track.Duration.ToFormattedString();

        PaceTitle = "Average Pace";
        PaceValue = Track.Pace.ToFormattedString();

        RankTitle = "Rank";
        RankValue = rank.ToString();
        RankCount = count.ToString();

        CaloriesTitle = "Calories Burned";
        CaloriesValue = Track.Calories.ToString();
    }
}