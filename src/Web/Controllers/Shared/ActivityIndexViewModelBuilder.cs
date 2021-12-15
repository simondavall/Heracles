using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Heracles.Application.Extensions;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Identity;
using Heracles.Web.Models;

namespace Heracles.Web.Controllers.Shared
{
    public class ActivityIndexViewModelBuilder
    {
        private readonly IActivityService _activityService;

        public ActivityIndexViewModelBuilder(IActivityService activityService)
        {
            _activityService = activityService;
        }

        public async Task<IndexViewModel> GetIndexViewModel(Track track, string siteRoot, ApplicationUser user)
        {
            return new IndexViewModel
            {
                SubNavigationViewModel = await GetSubNavViewModel(user),
                ActivityListViewModel = await GetActivityListViewModel(track),
                ActivityTitleViewModel = GetActivityTitleViewModel(track),
                StatsBarViewModel = await GetStatsBarViewModel(track),
                MapAreaViewModel = new MapAreaViewModel { TrackId = track.Id, SiteRoot = siteRoot }
            };
        }

        private async Task<ActivityListViewModel> GetActivityListViewModel(Track track)
        {
            var monthlyActivitySummary = await _activityService.GetActivitiesSummaryByMonths(track);

            var selectedMonthIndex = monthlyActivitySummary.TakeWhile(summary => summary.Activities is null).Count();

            var activitiesSummary = monthlyActivitySummary.Select(x =>
                new ActivitiesByMonthViewModel
                {
                    ActivityCount = x.Count,
                    ActivityDate = new DateTime(x.ActivityYearMonth / 100, x.ActivityYearMonth % 100, 1),
                    Activities = x.Activities
                }).ToList();

            return new ActivityListViewModel { ActivityListMonths = activitiesSummary,  ActiveMonthTab = selectedMonthIndex, SelectedActivityId = track.Id.ToString() };
        }

        private ActivityTitleViewModel GetActivityTitleViewModel(Track track)
        {
            var model = new ActivityTitleViewModel()
            {
                Image = track.ActivityType.GetImagePath(),
                Title = $"{track.Time.DayOfWeek} {track.ActivityType.GetTitleText()}",
                Date = track.Time.ToString("MMM dd, yyyy - HH:mm")
            };
            return model;
        }

        private async Task<StatsBarViewModel> GetStatsBarViewModel(Track track)
        {
            var (rank, count) = await _activityService.GetActivityRank(track);
            var model = new StatsBarViewModel
            {
                DistanceTitle = "km",
                DistanceValue = track.Distance.ToString("0.00", CultureInfo.InvariantCulture),
                DurationTitle = "Duration",
                DurationValue = track.Duration.ToFormattedString(),
                PaceTitle = "Average Pace",
                PaceValue = track.Pace.ToFormattedString(),
                RankTitle = "Rank",
                RankValue = rank.ToString(),
                RankCount = count.ToString(),
                CaloriesTitle = "Calories Burned",
                CaloriesValue = track.Calories.ToString()
            };
            return model;
        }

        private async Task<SubNavigationViewModel> GetSubNavViewModel(ApplicationUser user)
        {
            var firstEverActivity = await _activityService.GetFirstEverActivityAsync();
            var dateOfEarliestActivity = firstEverActivity?.Time ?? DateTime.UtcNow;

            var subNav = new SubNavigationViewModel
            {
                ActiveSince = $"Active since {dateOfEarliestActivity:MMM, yyyy}",
                Username = user is null ? "Not Logged In" : user.DisplayName
            };

            return subNav;
        }
    }
}
