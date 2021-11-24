using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Heracles.Application.Extensions;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
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

        public async Task<IndexViewModel> GetIndexViewModel(Track track, string siteRoot)
        {
            return new IndexViewModel
            {
                SubNavigationViewModel = new SubNavigationViewModel()
                {
                    ActiveSince = "Active since Sep, 2009",
                    Username = "Simon Da Vall"
                },
                ActivityListViewModel = await GetActivityListViewModel(),
                ActivityTitleViewModel = GetActivityTitleViewModel(track),
                StatsBarViewModel = GetStatsBarViewModel(track),
                MapAreaViewModel = new MapAreaViewModel { TrackId = track.Id, SiteRoot = siteRoot }
            };
        }

        private async Task<ActivityListViewModel> GetActivityListViewModel()
        {
            var monthlyActivitySummary = await _activityService.GetActivitiesSummaryByMonths();

            var activitiesSummary = monthlyActivitySummary.Select(x =>
                new ActivitiesByMonthViewModel
                {
                    ActivityCount = x.Count,
                    ActivityDate = new DateTime(x.ActivityYearMonth / 100, x.ActivityYearMonth % 100, 1)
                }).ToList();

            return new ActivityListViewModel { ActivityListMonths = activitiesSummary };
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

        private StatsBarViewModel GetStatsBarViewModel(Track track)
        {
            var model = new StatsBarViewModel
            {
                DistanceTitle = "km",
                DistanceValue = track.Distance.ToString("0.00", CultureInfo.InvariantCulture),
                DurationTitle = "Duration",
                DurationValue = track.Duration.ToFormattedString(),
                PaceTitle = "Average Pace",
                PaceValue = track.Pace.ToFormattedString(),
                CaloriesTitle = "Calories Burned",
                CaloriesValue = track.Calories.ToString()
            };
            return model;
        }
    }
}
