using System;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Heracles.Application.Interfaces;
using Heracles.Application.Resources;
using Heracles.Application.Services.Import;
using Heracles.Web.Models;


namespace Heracles.Web.Controllers
{
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;
        private readonly ITrackRepository _trackRepository;
        private readonly IImportService _importService;

        public ImportController(IImportService importService, ITrackRepository trackRepository, ILogger<ImportController> logger)
        {
            _importService = importService;
            _trackRepository = trackRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = BuildImportViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var importFilesResult = await _importService.ImportTracksFromGpxFilesAsync(Request.Form.Files);

            try
            {
                await _trackRepository.SaveImportedFilesAsync(importFilesResult, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to save track details to db with message: {e.Message}");
                ModelState.AddModelError(string.Empty, ImportServiceStrings.FailedToSaveImportedFiles);
                return View("Index", BuildImportViewModel());
            }
            var model = BuildImportViewModel();
            model.FilesFailed = importFilesResult.FailedFiles;
            model.FilesImported = importFilesResult.ImportedFiles.Count;
            model.ImportExecuted = true;

            return View("Index", model);
        }

        private ImportViewModel BuildImportViewModel()
        {
            var model = new ImportViewModel
            {
                SubNavigationViewModel = new SubNavigationViewModel()
                {
                    ActiveSince = "Active since Sep, 2009",
                    Username = "Simon Da Vall"
                }
            };
            model.SubNavigationViewModel.SetSelectedTab(SubNavTab.Import);
            return model;
        }
    }
}
