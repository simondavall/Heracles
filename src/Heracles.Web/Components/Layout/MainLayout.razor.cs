using System.Security.Claims;
using Heracles.Web.Components.Features.UserState;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Heracles.Web.Components.Layout;

public partial class MainLayout
{
    [Inject]
    private UserStateService UserStateService { get; set; } = null!;
    
    private MudThemeProvider? _themeProvider;
    private bool _isDarkMode;

    private string ThemeToggleIcon => _isDarkMode
        ? Icons.Material.Filled.LightMode
        : Icons.Material.Filled.DarkMode;

    private string ThemeToggleText => _isDarkMode
        ? "Switch to light mode"
        : "Switch to dark mode";

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender || _themeProvider is null)
            return;

        await UserStateService.LoadAsync();
        _isDarkMode = UserStateService.State.IsDarkMode ?? await _themeProvider.GetSystemDarkModeAsync();

        await _themeProvider.WatchSystemDarkModeAsync(OnSystemThemeChangedAsync);
        
        StateHasChanged();
    }

    private Task OnSystemThemeChangedAsync(bool isDarkMode) {
        if (UserStateService.State.IsDarkMode is not null)
            return Task.CompletedTask;

        _isDarkMode = isDarkMode;

        return InvokeAsync(StateHasChanged);
    }

    private async Task ToggleThemeAsync() {
        _isDarkMode = !_isDarkMode;

        UserStateService.State.IsDarkMode = _isDarkMode;

        await UserStateService.SaveAsync();
    }

    private static string DisplayName(ClaimsPrincipal user) {
        return user.FindFirst("display_name")?.Value
               ?? user.Identity?.Name
               ?? string.Empty;
    }
}