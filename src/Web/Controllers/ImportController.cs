using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Heracles.Application.GpxTrackAggregate;
using Heracles.Application.Interfaces;
using Heracles.Domain.Interfaces;
using Heracles.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Heracles.Web.Controllers
{
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;
        private readonly IRepository<TrackAggregate> _trackRepository;
        private readonly IGpxService _gpxService;

        public ImportController(ILogger<ImportController> logger, IRepository<TrackAggregate> gpxRepository, IGpxService gpxService)
        {
            _logger = logger;
            _trackRepository = gpxRepository;
            _gpxService = gpxService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ImportViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var importViewModel = new ImportViewModel { ImportExecuted = true };

            if (Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    if (file.FileName.EndsWith(".gpx"))
                    {
                        await LoadContentsOfGpxFile(file, importViewModel);
                    }
                }
            }
            return View("Index", importViewModel);
        }

        private async Task LoadContentsOfGpxFile(IFormFile file, ImportViewModel importViewModel)
        {
            try
            {
                var trackAggregate = await _gpxService.LoadLoadContentsOfGpxFile(file);
                if (trackAggregate != null)
                {
                    await _trackRepository.AddAsync(trackAggregate);
                    importViewModel.FilesImported++;
                }
                else
                {
                    importViewModel.FilesFailed.Add(new FailedFile
                    {
                        Filename = file.FileName,
                        ErrorMessage = "File could not be processed."
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to import file {file.FileName} with message: {e.Message}");
                importViewModel.FilesFailed.Add(new FailedFile
                {
                    Filename = file.FileName,
                    ErrorMessage = e.Message
                });
            }
        }
    }
}
