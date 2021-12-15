using System;
using System.Threading.Tasks;
using Heracles.Application.Enums;
using Heracles.Application.Interfaces;
using Heracles.Infrastructure.Identity;
using Heracles.Web.Controllers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Heracles.Web.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ActivityController> _logger;
        private readonly ActivityIndexViewModelBuilder _activityViewModelBuilder;

        public ActivityController(IActivityService activityService, UserManager<ApplicationUser> userManager, ILogger<ActivityController> logger)
        {
            _activityService = activityService;
            _userManager = userManager;
            _activityViewModelBuilder = new ActivityIndexViewModelBuilder(activityService);
            _logger = logger;
        }

        [Route("[controller]/{id}")]
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            if (!Guid.TryParse(id, out var trackId))
            {
                return RedirectToAction("Index", "Home");
            }

            var track = await _activityService.GetActivity(trackId);
            var siteRoot = $"{Request.Scheme}://{Request.Host}";
            var user = await _userManager.GetUserAsync(User);

            var model = await  _activityViewModelBuilder.GetIndexViewModel(track, siteRoot, user);
            model.SubNavigationViewModel.SetSelectedTab(SubNavTab.Dashboard);

            return View(model);
        }
    }
}
