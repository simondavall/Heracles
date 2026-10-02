using Heracles.Application.Activities;
using Heracles.Application.Data;
using Heracles.Web.Components.Features.ActivityDetails;
using Heracles.Web.Components.Features.UserState;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Pages;

public partial class Home
{
    [Inject]
    private IActivityService ActivityService { get; set; } = null!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;
    [Inject]
    private UserStateService UserStateService { get; set; } = null!;
    [Inject]
    private ActivityDetailsInteractionService InteractionService { get; set; } = null!;
    
    [Parameter]
    public Guid? ActivityId { get; set; }

    private Track? _activity;
    private bool _initialActivityLoaded;

    protected override async Task OnParametersSetAsync() {
        if (!ActivityId.HasValue)
            return;

        InteractionService.Clear();

        _activity = await ActivityService.GetActivityAsync(ActivityId.Value);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender || ActivityId.HasValue || _initialActivityLoaded)
            return;

        _initialActivityLoaded = true;

        await UserStateService.LoadAsync();

        _activity = await ActivityService.GetMostRecentActivityAsync(UserStateService.State.ActivityType);

        if (_activity is not null)
            NavigationManager.NavigateTo($"/activity/{_activity.Id}", replace: true);
    }
}