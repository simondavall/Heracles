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
using Microsoft.AspNetCore.Authorization;


namespace Heracles.Web.Controllers
{
    [Authorize]
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
            var model = BuildImportViewModel(Guid.NewGuid());
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Upload(string processId)
        {
            if (!Guid.TryParse(processId, out var processGuid))
            {
                _logger.LogError($"Upload did not contain a valid Guid for processId");
                throw new ArgumentException("ProcessId must be a valid Guid.", nameof(processId));
            }
            
            var importFilesResult = await _importService.ImportTracksFromGpxFilesAsync(Request.Form.Files);

            try
            {
                //await _trackRepository.SaveImportedFilesAsync(importFilesResult, CancellationToken.None);
                await _trackRepository.SaveImportedFilesAsync(importFilesResult, processGuid, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to save track details to db with message: {e.Message}");
                ModelState.AddModelError(string.Empty, ImportServiceStrings.FailedToSaveImportedFiles);
                return View("Index", BuildImportViewModel(processGuid));
            }
            var model = BuildImportViewModel(processGuid);
            model.FilesFailed = importFilesResult.FailedFiles;
            model.FilesImported = importFilesResult.ImportedFiles.Count;
            model.ImportExecuted = true;

            return View("Index", model);
        }

        private ImportViewModel BuildImportViewModel(Guid processId)
        {
            var model = new ImportViewModel
            {
                SubNavigationViewModel = new SubNavigationViewModel()
                {
                    ActiveSince = "Active since Sep, 2009",
                    Username = "Simon Da Vall"
                }
            };
            model.ProcessId = processId;
            model.SubNavigationViewModel.SetSelectedTab(SubNavTab.Import);
            return model;
        }
    }
}
