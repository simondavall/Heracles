using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Pages;

public partial class Home
{
    [Inject]
    private IActivityService ActivityService { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter]
    public Guid? ActivityId { get; set; }

    private Track? _activity;

    protected override async Task OnParametersSetAsync()
    {
        if (ActivityId.HasValue)
        {
            _activity = await ActivityService.GetActivityAsync(ActivityId.Value);
            return;
        }

        _activity = await ActivityService.GetMostRecentActivityAsync();

        if (_activity is not null)
            NavigationManager.NavigateTo($"/activity/{_activity.Id}", replace: true);
    }
}