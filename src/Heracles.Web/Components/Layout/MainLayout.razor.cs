using System.Security.Claims;
using Heracles.Web.Components.Theme;
using MudBlazor;

namespace Heracles.Web.Components.Layout;

public partial class MainLayout
{
    private MudThemeProvider? _themeProvider;
    private ThemeMode _themeMode = ThemeMode.System;
    private bool _isDarkMode;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || _themeProvider is null)
            return;

        _isDarkMode = await _themeProvider.GetSystemDarkModeAsync();

        await _themeProvider.WatchSystemDarkModeAsync(OnSystemThemeChangedAsync);

        StateHasChanged();
    }

    private Task OnSystemThemeChangedAsync(bool isDarkMode)
    {
        if (_themeMode != ThemeMode.System)
            return Task.CompletedTask;

        _isDarkMode = isDarkMode;

        return InvokeAsync(StateHasChanged);
    }

    private async Task SetThemeModeAsync(ThemeMode themeMode)
    {
        _themeMode = themeMode;

        _isDarkMode = themeMode switch
        {
            ThemeMode.Light => false,
            ThemeMode.Dark => true,
            ThemeMode.System when _themeProvider is not null =>
                await _themeProvider.GetSystemDarkModeAsync(),
            _ => false
        };
    }
    
    private static string DisplayName(ClaimsPrincipal user)
    {
        return user.FindFirst("display_name")?.Value
               ?? user.Identity?.Name
               ?? string.Empty;
    }
}