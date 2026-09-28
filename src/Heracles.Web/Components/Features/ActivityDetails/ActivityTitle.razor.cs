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

    private string WeatherIcon => _weather?.Conditions switch {
        "Clear sky" => Icons.Material.Filled.WbSunny,
        "Mainly clear" => Icons.Material.Filled.WbSunny,
        "Partly cloudy" => Icons.Material.Filled.WbCloudy,
        "Overcast" => Icons.Material.Filled.Cloud,
        "Fog" => Icons.Material.Filled.Cloud,
        "Depositing rime fog" => Icons.Material.Filled.Cloud,
        "Light drizzle" => Icons.Material.Filled.WaterDrop,
        "Moderate drizzle" => Icons.Material.Filled.WaterDrop,
        "Dense drizzle" => Icons.Material.Filled.WaterDrop,
        "Light freezing drizzle" => Icons.Material.Filled.AcUnit,
        "Dense freezing drizzle" => Icons.Material.Filled.AcUnit,
        "Slight rain" => Icons.Material.Filled.WaterDrop,
        "Moderate rain" => Icons.Material.Filled.WaterDrop,
        "Heavy rain" => Icons.Material.Filled.WaterDrop,
        "Light freezing rain" => Icons.Material.Filled.AcUnit,
        "Heavy freezing rain" => Icons.Material.Filled.AcUnit,
        "Slight snowfall" => Icons.Material.Filled.AcUnit,
        "Moderate snowfall" => Icons.Material.Filled.AcUnit,
        "Heavy snowfall" => Icons.Material.Filled.AcUnit,
        "Snow grains" => Icons.Material.Filled.AcUnit,
        "Slight rain showers" => Icons.Material.Filled.WaterDrop,
        "Moderate rain showers" => Icons.Material.Filled.WaterDrop,
        "Violent rain showers" => Icons.Material.Filled.WaterDrop,
        "Slight snow showers" => Icons.Material.Filled.AcUnit,
        "Heavy snow showers" => Icons.Material.Filled.AcUnit,
        "Thunderstorm" => Icons.Material.Filled.Storm,
        "Thunderstorm with slight hail" => Icons.Material.Filled.Storm,
        "Heavy thunderstorm" => Icons.Material.Filled.Storm,
        "Thunderstorm with heavy hail" => Icons.Material.Filled.Storm,
        "Unknown" => Icons.Material.Filled.Cloud,
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