using Heracles.Application.Extensions;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.Activity;

public partial class ActivityTitle : ComponentBase
{
    [Parameter]
    public required Track Track { get; set; }

    private string? Title { get; set; }
    private string? Image { get; set; }
    private string? Date { get; set; }

    protected override void OnParametersSet() {
        Image = Track.ActivityType.GetImagePath();
        Title = $"{Track.Time.DayOfWeek} {Track.ActivityType.GetTitleText()}";
        Date = Track.Time.ToString("MMM dd, yyyy - HH:mm");
    }
}