using System.Security.Claims;
using Heracles.Application.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.Shared;

public partial class SubNavigation : ComponentBase
{
    [Inject]
    private IActivityService ActivityService { get; set; } = default!;
    
    [Parameter]
    public SubNavTab SelectedTab { get; set; }

    private string? ActiveSince { get; set; }
    private string? Username { get; set; }
    
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender)
            return;

        var firstEverActivity = await ActivityService.GetFirstEverActivityAsync();
        var dateOfEarliestActivity = firstEverActivity?.Time ?? DateTime.UtcNow;

        ActiveSince = $"Active since {dateOfEarliestActivity:MMM, yyyy}";
        Username = "<Username>";

        StateHasChanged();
    }

    private string GetTabClass(SubNavTab tab)
    {
        return tab == SelectedTab ? "selected" : "";
    }
    
    private static string DisplayName(ClaimsPrincipal user) {
        return user.FindFirst("display_name")?.Value
               ?? user.Identity?.Name
               ?? string.Empty;
    }
}