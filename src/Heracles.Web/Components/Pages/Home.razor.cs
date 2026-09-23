using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Pages;

public partial class Home
{
    [Inject]
    private IActivityService ActivityService { get; set; } = null!;

    [Parameter]
    public Guid? ActivityId { get; set; }

    private Track? _activity;

    protected override async Task OnParametersSetAsync()
    {
        _activity = ActivityId.HasValue
            ? await ActivityService.GetActivityAsync(ActivityId.Value)
            : null;
    }
}