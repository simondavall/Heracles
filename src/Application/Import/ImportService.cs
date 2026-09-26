#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.Data;
// using Heracles.Application.Exceptions;
using Heracles.Application.Import.Progress;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace Heracles.Application.Import;

public class ImportService : IImportService
{
    private readonly IGpxService _gpxService;
    private readonly ITrackRepository _trackRepository;
    private readonly ILogger<ImportService> _logger;

    public ImportService(IGpxService gpxService, ITrackRepository trackRepository, ILogger<ImportService> logger) {
        _gpxService = gpxService;
        _trackRepository = trackRepository;
        _logger = logger;
    }

    public async Task<ImportFilesResult> ImportTracksFromGpxFilesAsync(IReadOnlyList<IBrowserFile> files,
        long maxAllowedSize,
        Action<decimal>? progress = null,
        CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentOutOfRangeException.ThrowIfZero(files.Count, nameof(files));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAllowedSize);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(files.Sum(file => file.Size), maxAllowedSize, nameof(files));

        var result = new ImportFilesResult();

        var existingTracks = await ExistingTracks.CreateAsync(_trackRepository);

        var importProgress = new TrackImportProgress(progress);

        for (var index = 0; index < files.Count; index++) {
            cancellationToken.ThrowIfCancellationRequested();

            var file = files[index];
            await ProcessFileAsync(file, maxAllowedSize, existingTracks, result, cancellationToken);

            importProgress.TrackProgressMethod(ProgressHelper.GetProgress(index + 1, files.Count));
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (result.Tracks.Count > 0) {
            importProgress.UpdateWithProcessedFileData(result);
            await _trackRepository.SaveImportedFilesAsync(result, importProgress, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
        importProgress.Complete();

        return result;
    }

    private async Task ProcessFileAsync(IBrowserFile file,
        long maxAllowedSize,
        IExistingTracks existingTracks,
        ImportFilesResult result,
        CancellationToken cancellationToken) {
        if (!file.Name.EndsWith(".gpx", StringComparison.OrdinalIgnoreCase)) {
            AddFailure(result, file.Name, ImportServiceStrings.IncorrectFileExtension);
            return;
        }

        try {
            var track = await _gpxService.LoadContentsOfGpxFileAsync(file, maxAllowedSize, cancellationToken);
            ValidateTrackData(track);

            if (existingTracks.TrackExists(track.Name)) {
                AddFailure(result, file.Name, ImportServiceStrings.DuplicateTrackRecord);
                return;
            }

            result.Tracks.Add(track);
            result.TrackSegments.AddRange(track.TrackSegments);

            foreach (var segment in track.TrackSegments) {
                result.TrackPoints.AddRange(segment.TrackPoints);
            }

            existingTracks.AddTrack(track.Name);

            result.ImportedFiles.Add(new FileResult(file.Name, ImportServiceStrings.ImportSuccess));
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested) {
            throw;
        }
        catch (ImportServiceException exception) {
            AddFailure(result, file.Name, exception.Message);
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Failed to process GPX file {FileName}", file.Name);
            AddFailure(result, file.Name, ImportServiceStrings.FileCouldNotBeProcessed);
        }
    }

    private static void AddFailure(ImportFilesResult result, string filename, string reason) {
        result.FailedFiles.Add(new FileResult(filename, reason));
    }

    private static void ValidateTrackData(Track? track) {
        if (track is null) {
            throw new ImportServiceException(ImportServiceStrings.FileCouldNotBeProcessed);
        }

        if (track.TrackSegments is null || track.TrackSegments.Count == 0) {
            throw new ImportServiceException(ImportServiceStrings.NoTrackSegmentsFound);
        }

        foreach (var segment in track.TrackSegments) {
            if (segment.TrackPoints is null || segment.TrackPoints.Count == 0) {
                throw new ImportServiceException(ImportServiceStrings.NoTrackPointsFound);
            }
        }
    }
}