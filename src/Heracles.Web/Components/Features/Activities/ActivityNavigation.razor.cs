using System.Globalization;
using Heracles.Application.Activities;
using Heracles.Application.Tracks;
using Heracles.Web.Components.Features.UserState;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.Activities;

public partial class ActivityNavigation
{
    [Inject]
    private IActivityService ActivityService { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private UserStateService UserStateService { get; set; } = null!;

    [Parameter]
    public Guid? ActivityId { get; set; }

    private Track? _currentActivity;
    private IList<ActivityListYear> _years = [];
    private IList<ActivityListMonth> _months = [];
    private List<ActivityListItem> _activities = [];
    private IList<ActivityType> _activityTypes = [];

    private ActivityType? _selectedActivityType;

    private int? _expandedYear;
    private int? _expandedMonth;
    private Guid? _selectedActivityId;

    private bool _isLoading = true;
    private bool _isLoadingMonth;
    private bool _initialized;

    protected override async Task OnParametersSetAsync() {
        if (!_initialized)
            return;

        await SetCurrentActivityAsync();
        await SetNavigationSelectionAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender)
            return;

        await UserStateService.LoadAsync();

        _activityTypes = await ActivityService.GetActivityTypesAsync();
        
        _selectedActivityType = UserStateService.State.ActivityType;
        if (_selectedActivityType.HasValue && !_activityTypes.Contains(_selectedActivityType.Value))
        {
            _selectedActivityType = null;
            UserStateService.State.ActivityType = null;
            await UserStateService.SaveAsync();
        }
        

        await SetCurrentActivityAsync();
        await LoadNavigationAsync();

        _initialized = true;
        _isLoading = false;

        StateHasChanged();
    }

    private async Task SetCurrentActivityAsync() {
        _currentActivity = ActivityId.HasValue
            ? await ActivityService.GetActivityAsync(ActivityId.Value)
            : null;

        _selectedActivityId = ActivityId;
    }

    private async Task LoadNavigationAsync() {
        _years = [];
        _months = [];
        _activities = [];
        _expandedYear = null;
        _expandedMonth = null;

        var referenceActivity = _currentActivity;

        if (referenceActivity is null) {
            referenceActivity = await ActivityService.GetMostRecentActivityAsync(_selectedActivityType);
        }

        if (referenceActivity is null)
            return;

        var yearsTask = ActivityService.GetActivitiesSummaryByYearAsync(referenceActivity, _selectedActivityType);
        var monthsTask = ActivityService.GetActivitiesSummaryByMonthsAsync(referenceActivity, _selectedActivityType);

        await Task.WhenAll(yearsTask, monthsTask);

        _years = await yearsTask;
        _months = await monthsTask;

        await SetNavigationSelectionAsync();
    }

    private async Task SetNavigationSelectionAsync() {
        _expandedYear = null;
        _expandedMonth = null;
        _activities = [];

        if (_currentActivity is null)
            return;

        if (_selectedActivityType.HasValue && _currentActivity.ActivityType != _selectedActivityType.Value)
            return;

        _expandedYear = _currentActivity.Time.Year;
        _expandedMonth = _currentActivity.Time.Year * 100 + _currentActivity.Time.Month;

        var currentMonth = _months.FirstOrDefault(x => x.ActivityYearMonth == _expandedMonth);

        if (currentMonth is not null)
            await LoadMonthActivitiesAsync(currentMonth);
    }

    private async Task ActivityTypeChangedAsync(ActivityType? activityType) {
        if (_selectedActivityType == activityType)
            return;

        _selectedActivityType = activityType;

        UserStateService.State.ActivityType = activityType;
        await UserStateService.SaveAsync();

        _isLoading = true;

        try {
            await LoadNavigationAsync();
        }
        finally {
            _isLoading = false;
        }
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

            if (_expandedMonth.HasValue && _expandedMonth.Value / 100 != year) {
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

    private async Task MonthExpandedChangedAsync(ActivityListMonth month, bool expanded) {
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
                _selectedActivityId,
                _selectedActivityType);
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