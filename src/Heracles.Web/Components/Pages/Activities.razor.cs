using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Pages;

public partial class Activities : ComponentBase
{
    [Inject]
    public IActivityService ActivityService { get; set; } = default!;
    [Parameter]
    public Guid? Id { get; set; }

    private Track? Track { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Id is null)
        {
            Track = await ActivityService.GetMostRecentActivityAsync();
        }
        else
        {
            Track = await ActivityService.GetActivityAsync(Id.Value);
        }
    }
    
    protected override async Task OnInitializedAsync() {
        // todo: need to figure out where these variables are being used and replace then with better options.
        //var siteRoot = $"{Request.Scheme}://{Request.Host}";
        //var user = await _userManager.GetUserAsync(User);
        //var user = null;
    }
}