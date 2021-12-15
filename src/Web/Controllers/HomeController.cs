using Heracles.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Heracles.Application.Enums;
using Heracles.Application.Extensions;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Heracles.Web.Controllers.Shared;

namespace Heracles.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<HomeController> _logger;
        private readonly ActivityIndexViewModelBuilder _activityViewModelBuilder;

        public HomeController(IActivityService activityService, ILogger<HomeController> logger)
        {
            _activityService = activityService;
            _activityViewModelBuilder = new ActivityIndexViewModelBuilder(activityService);
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var track = await _activityService.GetMostRecentActivity();
            var siteRoot = $"{Request.Scheme}://{Request.Host}";
            var model = await _activityViewModelBuilder.GetIndexViewModel(track, siteRoot);
            model.SubNavigationViewModel.SetSelectedTab(SubNavTab.Dashboard);
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

        
    }
}
