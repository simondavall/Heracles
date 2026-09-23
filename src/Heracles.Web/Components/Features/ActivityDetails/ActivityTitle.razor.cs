using Heracles.Application.Enums;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.ActivityDetails;

public partial class ActivityTitle
{
    [Parameter]
    public Track Track { get; set; } = null!;

    private string ImagePath => Track.ActivityType switch
    {
        ActivityType.Cycling => "/images/icon-cycling.png",
        _ => "/images/icon-running.png"
    };

    private string ActivityTitleText => Track.ActivityType switch
    {
        ActivityType.Running => "Run",
        ActivityType.Cycling => "Bike Ride",
        _ => string.Empty
    };

    private string Title => $"{Track.Time.DayOfWeek} {ActivityTitleText}";

    private string Date => Track.Time.ToString("MMM dd, yyyy - HH:mm");
}