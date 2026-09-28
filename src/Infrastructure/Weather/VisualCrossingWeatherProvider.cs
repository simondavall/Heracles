using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Heracles.Application.Configuration;
using Heracles.Application.Weather;
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Heracles.Infrastructure.Weather;

public sealed class VisualCrossingWeatherProvider : IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiSettings _settings;

    public VisualCrossingWeatherProvider(HttpClient httpClient, WeatherApiSettings settings) { 
        _httpClient = httpClient; 
        _settings = settings;
    }

    public async Task<WeatherObservation?> GetHistoricalWeatherAsync(double latitude, double longitude, DateTime datetime, 
        CancellationToken cancellationToken = default) {
        var utc = GetNormalizedUtc(datetime);
        var location = string.Create(CultureInfo.InvariantCulture, $"{latitude:F6},{longitude:F6}");
        var endpoint = _settings.Uri.ToString().TrimEnd('/');

        var requestUri =
            $"{endpoint}/{location}/"
            + $"{utc:yyyy-MM-ddTHH:mm:ss}/"
            + "?unitGroup=metric"
            + "&include=hours"
            + "&contentType=json"
            + "&timezone=Z"
            + "&iconSet=icons2"
            + "&elements=datetime,datetimeEpoch,temp,feelslike,conditions,icon"
            + $"&key={Uri.EscapeDataString(_settings.Key)}";

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VisualCrossingResponse>(cancellationToken);
        if (result?.Days is not { Count: > 0 })
            return null;

        var targetEpoch = new DateTimeOffset(utc).ToUnixTimeSeconds();

        var observation = result.Days[0].Hours?.SingleOrDefault(h => h.DatetimeEpoch == targetEpoch);
        if (observation?.DatetimeEpoch is null)
            return null;

        return new WeatherObservation(
            observation.Temp,
            observation.FeelsLike,
            observation.Conditions,
            MapWeatherCode(observation.Icon),
            DateTimeOffset
                .FromUnixTimeSeconds(observation.DatetimeEpoch.Value)
                .UtcDateTime);
    }

    private static DateTime GetNormalizedUtc(DateTime datetime) {
        var utc = datetime.Kind switch
        {
            DateTimeKind.Utc => datetime,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(datetime, DateTimeKind.Utc),
            _ => datetime.ToUniversalTime()
        };
        return new DateTime(
            utc.Year,
            utc.Month,
            utc.Day,
            utc.Hour,
            0,
            0,
            DateTimeKind.Utc);
    }
    
    private static WeatherCode MapWeatherCode(string? icon) => icon switch {
        "snow" => WeatherCode.Snow,
        "rain" => WeatherCode.Rain,
        "fog" => WeatherCode.Fog,
        "wind" => WeatherCode.Wind,
        "cloudy" => WeatherCode.Cloudy,
        "partly-cloudy-day" => WeatherCode.PartlyCloudyDay,
        "partly-cloudy-night" => WeatherCode.PartlyCloudyNight,
        "clear-day" => WeatherCode.ClearDay,
        "clear-night" => WeatherCode.ClearNight,
        "snow-showers-day" => WeatherCode.SnowShowersDay,
        "snow-showers-night" => WeatherCode.SnowShowersNight,
        "thunder-rain" => WeatherCode.ThunderRain,
        "thunder-showers-day" => WeatherCode.ThunderShowersDay,
        "thunder-showers-night" => WeatherCode.ThunderShowersNight,
        "showers-day" => WeatherCode.ShowersDay,
        "showers-night" => WeatherCode.ShowersNight,
        _ => WeatherCode.Unknown
    };
    
    private sealed class VisualCrossingResponse
    {
        [JsonPropertyName("days")]
        public List<VisualCrossingDay>? Days { get; init; }
    }

    private sealed class VisualCrossingDay
    {
        [JsonPropertyName("hours")]
        public List<VisualCrossingHour>? Hours { get; set; }
    }

    private sealed class VisualCrossingHour
    {
        [JsonPropertyName("datetimeEpoch")]
        public long? DatetimeEpoch { get; set; }

        [JsonPropertyName("temp")]
        public double? Temp { get; set; }

        [JsonPropertyName("feelslike")]
        public double? FeelsLike { get; set; }

        [JsonPropertyName("conditions")]
        public string? Conditions { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }
}