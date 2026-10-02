using Heracles.Application.Data;
using Heracles.Application.Speed;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heracles.Web.Components.Features.ActivityDetails.Speed;

public partial class ActivitySpeedChart : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private ISpeedService SpeedService { get; set; } = null!;

    [Parameter]
    public Track Track { get; set; } = null!;

    private ElementReference _chartContainer;

    private IJSObjectReference? _module;
    private IJSObjectReference? _chart;

    private SpeedChartData? _chartData;

    private Guid? _loadedTrackId;
    private Guid? _renderedTrackId;

    private bool _disposed;

    protected override async Task OnParametersSetAsync() {
        if (_loadedTrackId == Track.Id)
            return;

        var observations =
            await SpeedService.GetSpeedAsync(Track);

        if (_disposed)
            return;

        _chartData =
            new SpeedChartData(
                observations
                    .Select(
                        observation =>
                            new SpeedChartPoint(
                                observation.Distance,
                                observation.KilometresPerHour))
                    .ToArray());

        _loadedTrackId = Track.Id;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (_disposed || _chartData is null || _renderedTrackId == _loadedTrackId)
            return;

        _module ??=
            await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                "./Components/Features/ActivityDetails/Speed/ActivitySpeedChart.razor.js");

        if (_disposed)
            return;

        if (_chart is null) {
            _chart = await _module.InvokeAsync<IJSObjectReference>("createSpeedChart", _chartContainer, _chartData);
        }
        else {
            await _chart.InvokeVoidAsync("update", _chartData);
        }

        _renderedTrackId = _loadedTrackId;
    }

    public async ValueTask DisposeAsync() {
        _disposed = true;

        try {
            if (_chart is not null)
                await _chart.InvokeVoidAsync("dispose");

            if (_chart is not null)
                await _chart.DisposeAsync();

            if (_module is not null)
                await _module.DisposeAsync();
        }
        catch (JSDisconnectedException) {
            // The browser connection has already been terminated.
        }
    }
}