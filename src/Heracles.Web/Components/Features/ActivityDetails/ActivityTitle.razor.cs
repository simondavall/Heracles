using Heracles.Application.Activities;
using Heracles.Application.Data;
using Heracles.Application.Weather;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Heracles.Web.Components.Features.ActivityDetails;

public partial class ActivityTitle
{
    [Inject]
    private IWeatherService WeatherService { get; set; } = null!;

    [Parameter]
    public Track Track { get; set; } = null!;

    private ActivityWeather? _weather;
    private Guid? _loadedTrackId;

    private string ImagePath => Track.ActivityType switch {
        ActivityType.Cycling => "/images/icon-cycling.png",
        _ => "/images/icon-running.png"
    };

    private string ActivityTitleText => Track.ActivityType switch {
        ActivityType.Running => "Run",
        ActivityType.Cycling => "Bike Ride",
        _ => string.Empty
    };

    private string Title => $"{Track.Time.DayOfWeek} {ActivityTitleText}";

    private string Date => Track.Time.ToString("MMM dd, yyyy - HH:mm");

    private string Temperature => _weather?.FeelsLike is { } temperature ? $"{temperature:0}°C" : string.Empty;

    private string WeatherIcon => _weather?.WeatherCode switch {
        WeatherCode.ClearDay => Icons.Material.Filled.WbSunny,
        WeatherCode.ClearNight => Icons.Material.Filled.NightsStay,
        WeatherCode.Cloudy or WeatherCode.Fog => Icons.Material.Filled.Cloud,
        WeatherCode.Rain => Icons.Material.Filled.WaterDrop,
        WeatherCode.PartlyCloudyDay => Icons.Material.Filled.WbCloudy,
        WeatherCode.PartlyCloudyNight => Icons.Material.Filled.NightsStay,
        WeatherCode.ShowersDay => Icons.Material.Filled.WaterDrop,
        WeatherCode.ShowersNight => Icons.Material.Filled.WaterDrop,
        WeatherCode.Snow => Icons.Material.Filled.AcUnit,
        WeatherCode.SnowShowersDay => Icons.Material.Filled.AcUnit,
        WeatherCode.SnowShowersNight => Icons.Material.Filled.AcUnit,
        WeatherCode.ThunderRain => Icons.Material.Filled.Storm,
        WeatherCode.ThunderShowersDay => Icons.Material.Filled.Storm,
        WeatherCode.ThunderShowersNight => Icons.Material.Filled.Storm,
        WeatherCode.Wind => Icons.Material.Filled.Air,
        WeatherCode.Unknown => Icons.Material.Filled.QuestionMark,
        _ => Icons.Material.Filled.Cloud
    };
    
    protected override async Task OnParametersSetAsync() {
        if (_loadedTrackId == Track.Id)
            return;

        _loadedTrackId = Track.Id;
        _weather = null;

        var trackId = Track.Id;

        var weather = await WeatherService.GetActivityWeatherAsync(Track);

        // Ignore results belonging to an activity that
        // is no longer selected.
        if (_loadedTrackId != trackId)
            return;

        _weather = weather;
    }
}