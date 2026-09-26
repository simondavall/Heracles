using System.Globalization;
using Heracles.Application.Activities;
using Heracles.Application.Data;
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

        var currentMonth = _months.FirstOrDefault(
            x => x.ActivityYearMonth == _expandedMonth);

        if (currentMonth is not null)
            await LoadMonthActivitiesAsync(currentMonth);

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

    private bool IsYearExpanded(int year) {
        return _expandedYear == year;
    }

    private bool IsMonthExpanded(int activityYearMonth) {
        return _expandedMonth == activityYearMonth;
    }

    private Task YearExpandedChangedAsync(int year, bool expanded) {
        if (expanded) {
            _expandedYear = year;

            if (_expandedMonth.HasValue &&
                _expandedMonth.Value / 100 != year) {
                _expandedMonth = null;
                _activities = [];
            }
        }
        else if (_expandedYear == year) {
            _expandedYear = null;
            _expandedMonth = null;
            _activities = [];
        }

        return Task.CompletedTask;
    }

    private async Task MonthExpandedChangedAsync(
        ActivityListMonth month,
        bool expanded) {
        if (!expanded) {
            if (_expandedMonth == month.ActivityYearMonth) {
                _expandedMonth = null;
                _activities = [];
            }

            return;
        }

        _expandedMonth = month.ActivityYearMonth;
        await LoadMonthActivitiesAsync(month);
    }

    private async Task LoadMonthActivitiesAsync(ActivityListMonth month) {
        _isLoadingMonth = true;
        _activities = [];

        try {
            if (month.Activities.Count > 0) {
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
        return new DateTime(
            activityYearMonth / 100,
            activityYearMonth % 100,
            1);
    }

    private static string GetMonthName(int activityYearMonth) {
        return GetMonthDate(activityYearMonth)
            .ToString("MMMM", CultureInfo.InvariantCulture);
    }

    private static string GetActivityClass(bool selected) {
        return selected
            ? "activity-list-item selected"
            : "activity-list-item";
    }
    
    private string GetYearHeadingClass(int year) {
        return IsYearExpanded(year)
            ? "activity-heading activity-year-heading selected"
            : "activity-heading activity-year-heading";
    }

    private string GetMonthHeadingClass(int activityYearMonth) {
        return IsMonthExpanded(activityYearMonth)
            ? "activity-heading activity-month-heading selected"
            : "activity-heading activity-month-heading";
    }
}