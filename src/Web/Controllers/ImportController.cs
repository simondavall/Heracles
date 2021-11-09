using System;
using System.Threading;
using System.Threading.Tasks;
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
            return View(new ImportViewModel());
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
                return View("Index", new ImportViewModel());
            }

            var importViewModel = new ImportViewModel
            {
                FilesFailed = importFilesResult.FailedFiles,
                FilesImported = importFilesResult.ImportedFiles.Count,
                ImportExecuted = true
            };

            return View("Index", importViewModel);
        }
    }
}
