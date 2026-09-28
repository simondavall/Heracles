using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Heracles.Application.Configuration;
using Heracles.Application.Weather;
using Microsoft.Extensions.Logging;

namespace Heracles.Infrastructure.Weather;

public class OpenMeteoWeatherProvider : IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiSettings _settings;
    private readonly ILogger<OpenMeteoWeatherProvider> _logger;

    public OpenMeteoWeatherProvider(HttpClient httpClient, WeatherApiSettings settings, ILogger<OpenMeteoWeatherProvider> logger) {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task<WeatherObservation?> GetHistoricalWeatherAsync(double latitude,
        double longitude,
        DateTime timestampUtc,
        CancellationToken cancellationToken = default) {
        
        var utc = timestampUtc.Kind switch
        {
            DateTimeKind.Utc => timestampUtc,

            DateTimeKind.Unspecified =>
                DateTime.SpecifyKind(timestampUtc, DateTimeKind.Utc),

            _ => timestampUtc.ToUniversalTime()
        };

        var location = string.Create(CultureInfo.InvariantCulture, $"latitude={latitude:F6}&longitude={longitude:F6}");
        var date = utc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endpoint = _settings.Uri.ToString().TrimEnd('/');

        var requestUri =
            $"{endpoint}?{location}"
            + $"&start_date={date}&end_date={date}"
            + "&timezone=GMT"
            + "&hourly=weather_code,temperature_2m,apparent_temperature";

        _logger.LogInformation("Calling WeatherApi with {WeatherApiRequest}", requestUri);

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OpenMeteoResponse>(cancellationToken);

        var observation = result?.Hourly;

        if (observation is null || !IsValid(observation))
        {
            _logger.LogWarning("Weather API returned an invalid response for {TimestampUtc}", utc);
            return null;
        }

        // Open-Meteo returns hourly timestamps.
        // Match the hour containing the activity midpoint.
        var requestedHour = new DateTime(
            utc.Year,
            utc.Month,
            utc.Day,
            utc.Hour,
            0,
            0,
            DateTimeKind.Utc);


        var index = observation.Time.FindIndex(time => DateTime.SpecifyKind(time, DateTimeKind.Utc) == requestedHour);
        if (index < 0)
        {
            _logger.LogWarning("Weather API returned no observation for {RequestedHour}", requestedHour);
            return null;
        }

        var temperature = observation.Temperature[index];
        var feelsLike = observation.FeelsLike[index];
        var weatherCode = observation.WeatherCode[index];

        // An observation without weather information is not useful.
        if (temperature is null && feelsLike is null && weatherCode is null)
        {
            _logger.LogWarning("Weather observation for {RequestedHour} contains no data", requestedHour);
            return null;
        }

        var weatherObservation = new WeatherObservation(temperature, feelsLike, MapWeatherCode(weatherCode));
        
        _logger.LogInformation("Retrieved historical weather for {RequestedHour}: {WeatherObservation}", requestedHour, weatherObservation);
        
        return weatherObservation;
    }

    private sealed record OpenMeteoResponse
    {
        [JsonPropertyName("hourly")]
        public OpenMeteoHourly? Hourly { get; init; }
    }

    // ReSharper disable CollectionNeverUpdated.Local
    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed record OpenMeteoHourly
    {
        [JsonPropertyName("time")]
        public List<DateTime> Time { get; init; } = [];

        [JsonPropertyName("temperature_2m")]
        public List<double?> Temperature { get; init; } = [];

        [JsonPropertyName("apparent_temperature")]
        public List<double?> FeelsLike { get; init; } = [];

        [JsonPropertyName("weather_code")]
        public List<int?> WeatherCode { get; init; } = [];
    }
    // ReSharper restore CollectionNeverUpdated.Local

    private static bool IsValid(OpenMeteoHourly hourly)
    {
        var count = hourly.Time.Count;

        return count > 0
               && hourly.Temperature.Count == count
               && hourly.FeelsLike.Count == count
               && hourly.WeatherCode.Count == count;
    }

    private static string MapWeatherCode(int? code) => code switch {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 => "Fog",
        48 => "Depositing rime fog",
        51 => "Light drizzle",
        53 => "Moderate drizzle",
        55 => "Dense drizzle",
        56 => "Light freezing drizzle",
        57 => "Dense freezing drizzle",
        61 => "Slight rain",
        63 => "Moderate rain",
        65 => "Heavy rain",
        66 => "Light freezing rain",
        67 => "Heavy freezing rain",
        71 => "Slight snowfall",
        73 => "Moderate snowfall",
        75 => "Heavy snowfall",
        77 => "Snow grains",
        80 => "Slight rain showers",
        81 => "Moderate rain showers",
        82 => "Violent rain showers",
        85 => "Slight snow showers",
        86 => "Heavy snow showers",
        95 => "Thunderstorm",
        96 => "Thunderstorm with slight hail",
        97 => "Heavy thunderstorm",
        99 => "Thunderstorm with heavy hail",
        _ => "Unknown"
    };
}