using Heracles.Application.Configuration;
using Heracles.Application.Interfaces;
using Heracles.Application.Services.Import;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Heracles.Web.Components.Pages;

public partial class Import : IDisposable
{
    [Inject]
    private IImportService ImportService { get; set; } = default!;
    [Inject]
    private ImportSettings ImportSettings { get; set; } = default!;
    [Inject]
    private ILogger<Import> Logger { get; set; } = default!;

    private CancellationTokenSource? _cancellationTokenSource;
    private ImportFilesResult? _result;

    private bool _isImporting;
    private bool _importCompleted;
    private bool _disposed;

    private decimal _progress;

    private string? _errorMessage;
    private string? _informationMessage;

    private int _inputKey;

    private int _maximumFileCount;
    private int _maximumCombinedSizeMb;
    private long _maximumCombinedSize;

    protected override void OnInitialized() {
        _maximumFileCount = ImportSettings.MaximumFileCount;
        _maximumCombinedSizeMb = ImportSettings.MaximumCombinedSizeMb;

        _maximumCombinedSize = _maximumCombinedSizeMb * 1024L * 1024L;
    }

    private async Task OnFilesSelectedAsync(InputFileChangeEventArgs args) {
        if (_isImporting || _disposed)
            return;
        
        ResetResults();

        IReadOnlyList<IBrowserFile> files;

        try {
            files = args.GetMultipleFiles(_maximumFileCount);
        }
        catch (InvalidOperationException) {
            _errorMessage = $"You can import a maximum of {_maximumFileCount} files at once.";
            ResetInput();
            return;
        }

        if (files.Count == 0) {
            ResetInput();
            return;
        }

        if (files.Sum(file => file.Size) > _maximumCombinedSize) {
            _errorMessage = $"The combined file size cannot exceed {_maximumCombinedSizeMb} MB.";
            ResetInput();
            return;
        }

        if (files.Any(file => !file.Name.EndsWith(".gpx", StringComparison.OrdinalIgnoreCase))) {
            _errorMessage = "Only GPX files can be imported.";
            ResetInput();
            return;
        }

        _isImporting = true;
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        try {
            await InvokeAsync(StateHasChanged);

            var progress = new Action<decimal>(value => {
                if (_disposed)
                    return;
                
                _ = InvokeAsync(() => {
                    if (_disposed)
                        return;

                    _progress = Math.Clamp(value * 100M, 0M, 100M);

                    StateHasChanged();
                });
            });

            _result = await ImportService.ImportTracksFromGpxFilesAsync(files, _maximumCombinedSize, progress, cancellationToken);

            _progress = 100M;
            _importCompleted = true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            _informationMessage = "The import was cancelled.";
            _result = null;
        }
        catch (Exception exception) {
            Logger.LogError(exception, "Activity import failed");
            _errorMessage = "The import could not be completed. No files from this batch were saved.";
            _result = null;
        }
        finally {
            _isImporting = false;

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            ResetInput();

            if (!_disposed)
                await InvokeAsync(StateHasChanged);
        }
    }

    private void CancelImport() {
        if (!_isImporting)
            return;

        _informationMessage = "Cancelling import...";
        _cancellationTokenSource?.Cancel();
    }

    private void ResetResults() {
        _result = null;
        _progress = 0M;
        _importCompleted = false;

        _errorMessage = null;
        _informationMessage = null;
    }

    private void ResetInput() {
        _inputKey++;
    }

    public void Dispose() {
        _disposed = true;
        _cancellationTokenSource?.Cancel();
    }
}