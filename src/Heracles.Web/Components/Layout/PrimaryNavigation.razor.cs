using System.Globalization;
using System.Security.Claims;
using Heracles.Application.Activities;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Layout;

public partial class PrimaryNavigation
{
    [Inject]
    private IActivityService ActivityService { get; set; } = null!;

    private string ActiveSince { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync() {
        var firstEverActivity = await ActivityService.GetFirstEverActivityAsync();
        var dateOfEarliestActivity = firstEverActivity?.Time ?? DateTime.UtcNow;

        ActiveSince = dateOfEarliestActivity.ToString("MMM, yyyy", CultureInfo.InvariantCulture);
    }

    private static string DisplayName(ClaimsPrincipal user) {
        return user.FindFirst("display_name")?.Value
               ?? user.Identity?.Name
               ?? string.Empty;
    }
}