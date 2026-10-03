using System.Globalization;
using Heracles.Application.Activities;
using Heracles.Application.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heracles.Web.Components.Features.ActivityDetails.Ranking;

public partial class ActivityRanking : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public Track Track { get; set; } = null!;
    [Parameter]
    public ActivityRank ActivityRank { get; set; } = null!;
    [Parameter]
    public EventCallback Closed { get; set; }

    private ElementReference _panel;

    private IJSObjectReference? _module;
    private IJSObjectReference? _draggable;

    private bool _disposed;

    private string Title => $"Ranked List ({FormatDistance(ActivityRank.LowerDistance)} - {FormatDistance(ActivityRank.UpperDistance)})";

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (_disposed || _draggable is not null)
            return;

        _module ??=
            await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                "./Components/Features/ActivityDetails/Ranking/ActivityRanking.razor.js");

        if (_disposed)
            return;

        _draggable = await _module.InvokeAsync<IJSObjectReference>("makeDraggable", _panel);

        await _module.InvokeVoidAsync("centreCurrentActivity", _panel);
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

    private static string GetRowCssClass(bool isCurrent) {
        return isCurrent
            ? "activity-ranking-row activity-ranking-entry activity-ranking-current"
            : "activity-ranking-row activity-ranking-entry";
    }

    private static string FormatPace(TimeSpan pace) {
        return pace.TotalHours >= 1
            ? $"{pace.TotalHours:#0}:{pace.Minutes:00}:{pace.Seconds:00}"
            : $"{pace.Minutes:#0}:{pace.Seconds:00}";
    }

    private static string FormatDate(DateTime date) {
        return date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
    }

    private static string FormatDistance(double distance) {
        return $"{distance.ToString("0.00", CultureInfo.InvariantCulture)} km";
    }
}