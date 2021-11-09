using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Heracles.Application.Exceptions;
using Heracles.Application.Interfaces;
using Heracles.Application.Resources;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Heracles.Application.Services.Import
{
    public class ImportService : IImportService
    {
        private readonly IGpxService _gpxService;
        private readonly ITrackRepository _trackRepository;
        private IExistingTracks _existingTracks;
        private readonly ILogger<ImportService> _logger;

        public ImportService(IGpxService gpxService, ITrackRepository trackRepository, ILogger<ImportService> logger)
        {
            _gpxService = gpxService;
            _trackRepository = trackRepository;
            _logger = logger;
        }

        public async Task<ImportFilesResult> ImportTracksFromGpxFilesAsync(IFormFileCollection files, IExistingTracks existingTracks = null)
        {
            Guard.Against.Null(files, nameof(files));
            var result = new ImportFilesResult();

            _existingTracks = existingTracks ?? await ExistingTracks.CreateAsync(_trackRepository);

            foreach (var file in files)
            {
                if (!file.FileName.EndsWith(".gpx"))
                {
                    FailedFile(result, file, ImportServiceStrings.IncorrectFileExtension);
                    continue;
                }

                try
                {
                    var track = _gpxService.LoadContentsOfGpxFile(file);

                    ValidateTrackData(track);

                    result.Tracks.Add(track);
                    result.TrackSegments.AddRange(track.TrackSegments);
                    foreach (var segment in track.TrackSegments)
                    {
                        result.TrackPoints.AddRange(segment.TrackPoints);
                    }

                    _existingTracks.AddTrack(track.Name);
                }
                catch (ImportServiceException e)
                {
                    FailedFile(result, file, e.Message);
                    continue;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Failed to import file {file.FileName} with message: {e.Message}");
                    FailedFile(result, file, e.Message);
                    continue;
                }

                result.ImportedFiles.Add(new FileResult(file.FileName, ImportServiceStrings.ImportSuccess));
            }

            return result;
        }

        private static void FailedFile(ImportFilesResult result, IFormFile file, string errorMessage)
        {
            result.FailedFiles.Add(new FileResult(file.FileName, errorMessage));
        }

        private void ValidateTrackData(Track track)
        {
            if (track is null)
            {
                throw new ImportServiceException(ImportServiceStrings.FileCouldNotBeProcessed);
            }

            if (_existingTracks.TrackExists(track.Name))
            {
                throw new ImportServiceException(ImportServiceStrings.DuplicateTrackRecord);
            }

            if (track.TrackSegments is null || track.TrackSegments.Count == 0)
            {
                throw new ImportServiceException(ImportServiceStrings.NoTrackSegmentsFound);
            }

            foreach (var segment in track.TrackSegments)
            {
                if (segment.TrackPoints is null || segment.TrackPoints.Count == 0)
                {
                    throw new ImportServiceException(ImportServiceStrings.NoTrackPointsFound);
                }
            }
        }

    }
}
