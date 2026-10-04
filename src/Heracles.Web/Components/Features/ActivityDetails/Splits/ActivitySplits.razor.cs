using System.Globalization;
using Heracles.Application.Activities;
using Heracles.Application.Activities.Splits;
using Heracles.Application.Tracks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heracles.Web.Components.Features.ActivityDetails.Splits;

public partial class ActivitySplits : IAsyncDisposable
{
    [Inject]
    private ISplitService SplitService { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public Track Track { get; set; } = null!;
    [Parameter]
    public EventCallback Closed { get; set; }

    private ElementReference _panel;

    private IReadOnlyList<Split>? _splits;

    private IJSObjectReference? _module;
    private IJSObjectReference? _draggable;

    private Guid? _loadedTrackId;
    private bool _disposed;

    protected override async Task OnParametersSetAsync() {
        if (_loadedTrackId == Track.Id)
            return;

        _splits = null;

        var splits = await SplitService.GetSplitsAsync(Track);

        if (_disposed)
            return;

        _splits = splits;
        _loadedTrackId = Track.Id;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (_disposed || _draggable is not null)
            return;

        _module ??=
            await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                "./Components/Features/ActivityDetails/Splits/ActivitySplits.razor.js");

        if (_disposed)
            return;

        _draggable = await _module.InvokeAsync<IJSObjectReference>(
            "makeDraggable",
            _panel);
    }

    public async ValueTask DisposeAsync() {
        _disposed = true;

        try {
            if (_draggable is not null)
                await _draggable.InvokeVoidAsync("dispose");

            if (_draggable is not null)
                await _draggable.DisposeAsync();

            if (_module is not null)
                await _module.DisposeAsync();
        }
        catch (JSDisconnectedException) {
            // The browser connection has already been terminated.
        }
    }

    private async Task Close() {
        await Closed.InvokeAsync();
    }

    private static string FormatDistance(double distance) {
        return $"{distance.ToString("0.00", CultureInfo.InvariantCulture)} km";
    }

    private static string FormatSplitTime(double secondsPerKilometre) {
        var totalSeconds = (int)Math.Round(secondsPerKilometre);
        var minutes = totalSeconds / 60;
        var seconds = totalSeconds % 60;

        return $"{minutes}:{seconds:00}";
    }

    private static string FormatElevation(double elevation) {
        var roundedElevation = (int)Math.Round(elevation);

        return roundedElevation switch {
            > 0 => $"+{roundedElevation} m",
            _ => $"{roundedElevation} m"
        };
    }
}