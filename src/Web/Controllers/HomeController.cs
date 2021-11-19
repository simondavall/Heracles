using Heracles.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Heracles.Application.Extensions;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;

namespace Heracles.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IActivityService activityService, ILogger<HomeController> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var track = await _activityService.GetMostRecentActivity();

            var model = new IndexViewModel
            {
                SubNavigationViewModel = new SubNavigationViewModel()
                {
                    ActiveSince = "Active since Sep, 2009",
                    Username = "Simon Da Vall"
                },
                ActivityListViewModel = await GetActivityListViewModel(),
                ActivityTitleViewModel = GetActivityTitleViewModel(track)
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
    }
}
