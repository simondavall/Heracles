using System.Globalization;
using Heracles.Application.Entities;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.Activities;

public partial class ActivityNavigation
{
    [Inject]
    private IActivityService ActivityService { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter]
    public Guid? ActivityId { get; set; }

    private Track? _currentActivity;
    private IList<ActivityListYear> _years = [];
    private IList<ActivityListMonth> _months = [];
    private List<ActivityListItem> _activities = [];

    private int? _expandedYear;
    private int? _expandedMonth;
    private Guid? _selectedActivityId;

    private bool _isLoading = true;
    private bool _isLoadingMonth;

    protected override async Task OnInitializedAsync() {
        _currentActivity = await ActivityService.GetMostRecentActivityAsync();

        if (_currentActivity is null) {
            _isLoading = false;
            return;
        }

        _selectedActivityId = ActivityId ?? _currentActivity.Id;

        var yearsTask = ActivityService.GetActivitiesSummaryByYearAsync(_currentActivity);
        var monthsTask = ActivityService.GetActivitiesSummaryByMonthsAsync(_currentActivity);

        await Task.WhenAll(yearsTask, monthsTask);

        _years = await yearsTask;
        _months = await monthsTask;

        _expandedYear = _currentActivity.Time.Year;
        _expandedMonth = (_currentActivity.Time.Year * 100) + _currentActivity.Time.Month;

        var currentMonth = _months.FirstOrDefault(x => x.ActivityYearMonth == _expandedMonth);

        if (currentMonth?.Activities is not null) {
            _activities = currentMonth.Activities;
        }
        else if (currentMonth is not null) {
            _activities = await ActivityService.GetActivitiesByDateAsync(
                GetMonthDate(currentMonth.ActivityYearMonth),
                _selectedActivityId);
        }

        _isLoading = false;
    }

    protected override void OnParametersSet() {
        if (ActivityId.HasValue)
            _selectedActivityId = ActivityId.Value;
    }

    private IEnumerable<ActivityListMonth> GetMonths(int year) {
        return _months
            .Where(x => x.ActivityYearMonth / 100 == year)
            .OrderByDescending(x => x.ActivityYearMonth);
    }

    private Task ToggleYearAsync(int year) {
        _expandedYear = _expandedYear == year ? null : year;

        if (_expandedYear is null)
            _expandedMonth = null;

        return Task.CompletedTask;
    }

    private async Task ToggleMonthAsync(ActivityListMonth month) {
        if (_expandedMonth == month.ActivityYearMonth) {
            _expandedMonth = null;
            _activities = [];
            return;
        }

        _expandedMonth = month.ActivityYearMonth;
        _isLoadingMonth = true;
        _activities = [];

        try {
            if (month.Activities is not null) {
                _activities = month.Activities;
                return;
            }

            _activities = await ActivityService.GetActivitiesByDateAsync(
                GetMonthDate(month.ActivityYearMonth),
                _selectedActivityId);
        }
        finally {
            _isLoadingMonth = false;
        }
    }

    private void SelectActivity(Guid activityId) {
        _selectedActivityId = activityId;
        NavigationManager.NavigateTo($"/activity/{activityId}");
    }

    private static DateTime GetMonthDate(int activityYearMonth) {
        return new DateTime(activityYearMonth / 100, activityYearMonth % 100, 1);
    }

    private static string GetMonthName(int activityYearMonth) {
        return GetMonthDate(activityYearMonth).ToString("MMMM", CultureInfo.InvariantCulture);
    }

    private static string GetActivityClass(bool selected) {
        return selected
            ? "activity-list-item selected"
            : "activity-list-item";
    }
}