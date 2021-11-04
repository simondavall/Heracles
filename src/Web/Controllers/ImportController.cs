using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Data;
using Heracles.Web.Models;
using Microsoft.Extensions.DependencyInjection;


namespace Heracles.Web.Controllers
{
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;
        private readonly ITrackRepository _trackRepository;
        private readonly IGpxService _gpxService;
        private readonly IServiceProvider _services;
        private IList<string> _existingTracks;

        public ImportController(ILogger<ImportController> logger, ITrackRepository trackRepository, IGpxService gpxService, IServiceProvider services)
        {
            _logger = logger;
            _trackRepository = trackRepository;
            _gpxService = gpxService;
            _services = services;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ImportViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            _existingTracks = await _trackRepository.GetExistingTracksAsync();
            var importViewModel = new ImportViewModel { ImportExecuted = true };
            var trackAggregates = new List<Track>();
            var trackSegments = new List<TrackSegment>();
            var trackPoints = new List<TrackPoint>();

            if (Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    if (!file.FileName.EndsWith(".gpx")) 
                        continue;

                    var trackAggregate = await GetTracksAsync(file, importViewModel);
                    if (trackAggregate != null)
                    {
                        trackAggregates.Add(trackAggregate);
                        trackSegments.AddRange(trackAggregate.TrackSegments);
                        foreach (var trackSegment in trackAggregate.TrackSegments)
                        {
                            trackPoints.AddRange(trackSegment.TrackPoints);
                        }
                    }
                        
                }
                // TODO this should not be in the Import controller
                await using (var dbContext = _services.GetRequiredService<GpxDbContext>())
                {
                    var trackRepository = new TrackRepository(dbContext);
                    await trackRepository.BulkInsertAsync(trackAggregates, CancellationToken.None);
                    await trackRepository.BulkInsertAsync(trackSegments, CancellationToken.None);
                    await trackRepository.BulkInsertAsync(trackPoints, CancellationToken.None);
                }
            }

            return View("Index", importViewModel);
        }

        private async Task<Track> GetTracksAsync(IFormFile file, ImportViewModel importViewModel)
        {
            try
            {
                var trackAggregate = await _gpxService.LoadLoadContentsOfGpxFile(file);
                if (trackAggregate != null)
                {
                    if (!_existingTracks.Contains(trackAggregate.Name))
                    {
                        //  await _trackRepository.AddAsync(trackAggregate);
                        importViewModel.FilesImported++;
                        _existingTracks.Add(trackAggregate.Name);
                        return trackAggregate;
                    }
                    else
                    {
                        importViewModel.FilesFailed.Add(new FailedFile
                        {
                            Filename = file.FileName,
                            ErrorMessage = "Duplicate track record."
                        });
                    }
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

            return null;
        }
    }
}
