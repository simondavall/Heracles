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
    [Inject]
    private ActivityDetailsInteractionService InteractionService { get; set; } = null!;
    
    [Parameter]
    public Track Track { get; set; } = null!;

    private ElementReference _chartContainer;

    private IJSObjectReference? _module;
    private IJSObjectReference? _chart;

    private SpeedChartData? _chartData;
    private DotNetObjectReference<ActivitySpeedChart>? _dotNetReference;

    private Guid? _loadedTrackId;
    private Guid? _renderedTrackId;

    private bool _disposed;

    protected override void OnInitialized() {
        InteractionService.TrackPointChanged += OnTrackPointChanged;
    }
    
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
                                observation.TrackPointId,
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
            _dotNetReference ??= DotNetObjectReference.Create(this);

            _chart = await _module.InvokeAsync<IJSObjectReference>(
                "createSpeedChart",
                _chartContainer,
                _chartData,
                _dotNetReference);
        }
        else {
            await _chart.InvokeVoidAsync("update", _chartData);
        }

        await _chart.InvokeVoidAsync("selectTrackPoint", InteractionService.TrackPointId);
        
        _renderedTrackId = _loadedTrackId;
    }

    [JSInvokable]
    public void SelectTrackPoint(int trackPointId) {
        InteractionService.Select(trackPointId);
    }

    [JSInvokable]
    public void ClearTrackPoint() {
        InteractionService.Clear();
    }
    
    public async ValueTask DisposeAsync() {
        _disposed = true;

        InteractionService.TrackPointChanged -= OnTrackPointChanged;

        _dotNetReference?.Dispose();
        _dotNetReference = null;
        
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

    private void OnTrackPointChanged(int? trackPointId) {
        if (_disposed)
            return;

        _ = InvokeAsync(async () => {
            if (_disposed || _chart is null)
                return;

            try {
                await _chart.InvokeVoidAsync("selectTrackPoint", trackPointId);
            }
            catch (JSDisconnectedException) {
                // The browser connection has already been terminated.
            }
        });
    }
}