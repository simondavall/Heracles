using System;
using System.Threading.Tasks;
using Heracles.Application.Enums;
using Heracles.Application.Interfaces;
using Heracles.Web.Controllers.Shared;
using Heracles.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Heracles.Web.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<ActivityController> _logger;
        private readonly ActivityIndexViewModelBuilder _activityViewModelBuilder;

        public ActivityController(IActivityService activityService, ILogger<ActivityController> logger)
        {
            _activityService = activityService;
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
            var model = await  _activityViewModelBuilder.GetIndexViewModel(track, siteRoot);
            model.SubNavigationViewModel.SetSelectedTab(SubNavTab.Dashboard);
            return View(model);
        }
    }
}
