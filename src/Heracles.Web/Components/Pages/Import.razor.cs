using Heracles.Application.Interfaces;
using Heracles.Application.Resources;
using Heracles.Application.Services.Import;
using Heracles.Application.Services.Import.Progress;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Heracles.Web.Components.Pages;

public partial class Import : ComponentBase
{
    [Inject]
    private IImportService ImportService { get; set; } = default!;
    [Inject]
    private ITrackRepository TrackRepository { get; set; } = default!;
    [Inject]
    private IImportProgressService ProgressService { get; set; } = default!;
    [Inject]
    private ILogger<Import> Logger { get; set; } = default!;

    private readonly Guid _processId = Guid.NewGuid();
    private const int MaxFileCount = 100;
    private const long MaxFileSize = 1 * 1024 * 1024; // 1MB
    // Improbably the file import is too fast. Applying a small delay can provide the
    // user with a sense something is happening. Really need this to be externally 
    // configurable.
    private const int ImportDelay = 10; // ms
    
    private IList<FileResult> _filesFailed = [];
    private int _filesImported;
    private bool _importExecuted;
    private bool _isImporting;
    private decimal _progressPercentage;
    private string _selectedFileDescription = "No file selected";
    private string? _errorMessage;

    private async Task UploadFilesAsync(InputFileChangeEventArgs args) {
        var files = args.GetMultipleFiles(MaxFileCount);

        if (files.Count == 0)
            return;

        _selectedFileDescription =
            files.Count == 1
                ? files[0].Name
                : $"{files.Count} files selected";

        _filesFailed = [];
        _filesImported = 0;
        _importExecuted = false;
        _errorMessage = null;

        _isImporting = true;
        _progressPercentage = 0;

        try {
            await ImportFilesAsync(files);
        }
        finally {
            _isImporting = false;
        }
    }

    private async Task ImportFilesAsync(IReadOnlyList<IBrowserFile> files) {
        
        Logger.LogInformation($"Started importing {files.Count} files.");
        
        var trackProgress =
            new TrackImportProgress(
                ProgressService,
                _processId);

        try {
            var formFiles = await ConvertToFormFilesAsync(files);

            var result =
                await ImportService.ImportTracksFromGpxFilesAsync(
                    formFiles,
                    progress: ImportProgressChanged);

            trackProgress.UpdateWithProcessedFileData(result);

            await TrackRepository.SaveImportedFilesAsync(result, trackProgress, CancellationToken.None);

            _filesFailed = result.FailedFiles;
            _filesImported = result.ImportedFiles.Count;
            _importExecuted = true;
            _progressPercentage = 100;
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Failed to import activity files.");
            _errorMessage = ImportServiceStrings.FailedToSaveImportedFiles;
        }
    }

    private void ImportProgressChanged(decimal value) {
        _progressPercentage = value * 100;
        _ = InvokeAsync(StateHasChanged);
        
        Thread.Sleep(ImportDelay);
    }

    private string FormatDisplay(int count) => count == 1
        ? "1 file"
        : $"{count} files";

    private string FormatSuccessfullyImported() {
        var total = _filesImported + _filesFailed.Count;

        return total == 1
            ? $"{_filesImported}/{total} file"
            : $"{_filesImported}/{total} files";
    }
    
    private static async Task<IFormFileCollection> ConvertToFormFilesAsync(IReadOnlyList<IBrowserFile> files)
    {
        var formFiles = new FormFileCollection();

        foreach (var file in files)
        {
            var memoryStream = new MemoryStream();

            await using (var browserStream = file.OpenReadStream(MaxFileSize))
            {
                await browserStream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;

            var formFile = new FormFile(
                memoryStream,
                0,
                memoryStream.Length,
                file.Name,
                file.Name)
            {
                Headers = new HeaderDictionary(),
                ContentType = file.ContentType
            };

            formFiles.Add(formFile);
        }

        return formFiles;
    }
}