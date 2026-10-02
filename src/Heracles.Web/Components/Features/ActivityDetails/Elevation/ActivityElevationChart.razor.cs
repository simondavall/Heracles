using Heracles.Application.Configuration;
using Heracles.Application.Data;
using Heracles.Application.TrackPoints;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heracles.Web.Components.Features.ActivityDetails.Elevation;

public partial class ActivityElevationChart : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private ITrackPointDataService TrackPointDataService { get; set; } = null!;
    [Inject]
    private ElevationSettings ElevationSettings { get; set; } = null!;

    [Parameter]
    public Track Track { get; set; } = null!;

    private ElementReference _chartContainer;

    private IJSObjectReference? _module;
    private IJSObjectReference? _chart;

    private ElevationChartData? _chartData;

    private Guid? _loadedTrackId;
    private Guid? _renderedTrackId;

    private bool _disposed;

    protected override async Task OnParametersSetAsync() {
        if (_loadedTrackId == Track.Id)
            return;

        var trackPointData = await TrackPointDataService.GetAsync(Track);

        if (_disposed)
            return;

        var points =
            trackPointData
                .Select(data => new ElevationChartPoint(data.CumulativeDistance, data.Elevation))
                .ToArray();

        var (minimumElevation, maximumElevation) = CalculateElevationRange(points);

        _chartData = new ElevationChartData(points, minimumElevation, maximumElevation);

        _loadedTrackId = Track.Id;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (_disposed || _chartData is null || _renderedTrackId == _loadedTrackId)
            return;
        

        _module ??=
            await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                "./Components/Features/ActivityDetails/Elevation/ActivityElevationChart.razor.js");

        if (_disposed)
            return;

        if (_chart is null) 
            _chart = await _module.InvokeAsync<IJSObjectReference>("createElevationChart", _chartContainer, _chartData);
        else 
            await _chart.InvokeVoidAsync("update", _chartData);
        
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
    
    private (double? Minimum, double? Maximum) CalculateElevationRange(IReadOnlyList<ElevationChartPoint> points) {
        if (points.Count == 0)
            return (null, null);

        var minimum = points.Min(point => point.Elevation);
        var maximum = points.Max(point => point.Elevation);

        var actualRange = maximum - minimum;

        if (actualRange >= ElevationSettings.MinimumChartRange)
            return (minimum, maximum);

        var midpoint = (minimum + maximum) / 2;

        var halfRange = ElevationSettings.MinimumChartRange / 2.0;

        var low = midpoint - halfRange;
        var high = midpoint + halfRange;

        return (Math.Floor(low / 10) * 10, Math.Ceiling(high / 10) * 10);
        
        //return (midpoint - halfRange, midpoint + halfRange);
    }
}