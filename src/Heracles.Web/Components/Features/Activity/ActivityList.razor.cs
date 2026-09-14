using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.Activity;

public partial class ActivityList : ComponentBase
{
    [Inject]
    private IActivityService ActivityService { get; set; } = default!;

    [Parameter]
    public required Track Track { get; set; }

    private IList<MonthlyActivity> _monthlyActivityList = [];
    private int _activeMonthTab;
    private string _selectedActivityId = null!;
    
    protected override async Task OnInitializedAsync() {
        await LoadMonthlyActivitySummary();
    }

    private async Task LoadMonthlyActivitySummary() {
        var monthlyActivitySummary = await ActivityService.GetActivitiesSummaryByMonthsAsync(Track);
        _activeMonthTab = monthlyActivitySummary.TakeWhile(summary => summary.Activities is null).Count();

        _selectedActivityId = Track.Id.ToString();

        _monthlyActivityList = monthlyActivitySummary
            .Select(x =>
                new MonthlyActivity {
                    ActivityCount = x.Count,
                    ActivityDate = new DateTime(x.ActivityYearMonth / 100, x.ActivityYearMonth % 100, 1),
                    Activities = x.Activities
                })
            .ToList();
    }
}