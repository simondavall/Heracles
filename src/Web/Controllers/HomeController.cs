using Heracles.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using Heracles.Application.Enums;
using Heracles.Application.Interfaces;
using Heracles.Infrastructure.Identity;
using Heracles.Web.Controllers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Heracles.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly ActivityIndexViewModelBuilder _activityViewModelBuilder;

        public HomeController(IActivityService activityService, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            _activityService = activityService;
            _userManager = userManager;
            _activityViewModelBuilder = new ActivityIndexViewModelBuilder(activityService);
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var track = await _activityService.GetMostRecentActivityAsync();
            var siteRoot = $"{Request.Scheme}://{Request.Host}";
            var user = await _userManager.GetUserAsync(User);

            var model = await _activityViewModelBuilder.GetIndexViewModel(track, siteRoot, user);
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
