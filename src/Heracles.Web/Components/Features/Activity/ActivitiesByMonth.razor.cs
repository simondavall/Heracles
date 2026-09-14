using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.Activity;

public partial class ActivitiesByMonth : ComponentBase
{
    [Parameter]
    public MonthlyActivity MonthlyActivity { get; set; } = default!;
}