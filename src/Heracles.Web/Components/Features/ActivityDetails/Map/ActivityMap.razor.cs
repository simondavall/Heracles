using Heracles.Application.Configuration;
using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heracles.Web.Components.Features.ActivityDetails.Map;

public partial class ActivityMap : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private MapboxSettings MapboxSettings { get; set; } = null!;

    [Parameter]
    public Track Track { get; set; } = null!;

    private ElementReference _mapContainer;

    private IJSObjectReference? _module;
    private IJSObjectReference? _map;

    private MapData? _mapData;

    private bool _disposed;

    private Guid? _renderedActivityId;
    private bool _mapUpdateRequired;

    protected override void OnParametersSet() {
        if (_renderedActivityId == Track.Id)
            return;

        _mapData = CreateMapData(Track);
        _mapUpdateRequired = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (_disposed || !_mapUpdateRequired || _mapData is null)
            return;

        _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "./Components/Features/ActivityDetails/Map/ActivityMap.razor.js");

        if (_disposed)
            return;

        if (_map is null) {
            _map = await _module.InvokeAsync<IJSObjectReference>("createMap", _mapContainer, MapboxSettings.AccessToken, _mapData);
        }
        else {
            await _map.InvokeVoidAsync("update", _mapData);
        }

        _renderedActivityId = Track.Id;
        _mapUpdateRequired = false;
    }

    private static MapData CreateMapData(Track track)
    {
        var segments = track
            .TrackSegments
            .OrderBy(segment => segment.Seq)
            .Select(segment => new MapSegment(
                segment
                    .TrackPoints
                    .OrderBy(point => point.Seq)
                    .Select(point => new MapCoordinate(
                        point.Longitude,
                        point.Latitude))
                    .ToArray()))
            .Where(segment => segment.Coordinates.Count > 0)
            .ToArray();

        var distanceMarkers = DistanceMarkerCalculator.Calculate(track);

        return new MapData(segments, distanceMarkers);
    }

    public async ValueTask DisposeAsync() {
        _disposed = true;

        try {
            if (_map is not null)
                await _map.InvokeVoidAsync("dispose");

            if (_map is not null)
                await _map.DisposeAsync();

            if (_module is not null)
                await _module.DisposeAsync();
        }
        catch (JSDisconnectedException) {
            // The browser connection has already been terminated.
        }
    }
}