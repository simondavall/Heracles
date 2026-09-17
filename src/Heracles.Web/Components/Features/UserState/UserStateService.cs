using System.Text.Json;
using Microsoft.JSInterop;

namespace Heracles.Web.Components.Features.UserState;

public sealed class UserStateService(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private const string StorageKey = "heracles.userState";
    private const string ModulePath = "./js/userState.js";

    private readonly IJSRuntime _jsRuntime = jsRuntime;

    private IJSObjectReference? _module;

    public UserState State { get; private set; } = new();

    public async Task LoadAsync()
    {
        var module = await GetModuleAsync();
        var json = await module.InvokeAsync<string?>("get", StorageKey);

        if (string.IsNullOrWhiteSpace(json)) {
            State = new UserState();
            return;
        }

        try {
            State = JsonSerializer.Deserialize<UserState>(json) ?? new UserState();
        }
        catch (JsonException) {
            State = new UserState();
            await module.InvokeVoidAsync("remove", StorageKey);
        }
    }

    public async Task SaveAsync() {
        var module = await GetModuleAsync();
        var json = JsonSerializer.Serialize(State);
        await module.InvokeVoidAsync("set", StorageKey, json);
    }

    public async ValueTask DisposeAsync() {
        if (_module is not null)
            await _module.DisposeAsync();
    }

    private async Task<IJSObjectReference> GetModuleAsync() {
        _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
        return _module;
    }
}

public sealed class UserState
{
    public bool? IsDarkMode { get; set; }
}