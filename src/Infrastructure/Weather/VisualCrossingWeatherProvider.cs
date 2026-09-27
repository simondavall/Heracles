using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Heracles.Application.Configuration;
using Heracles.Application.Weather;

namespace Heracles.Infrastructure.Weather;

public sealed class VisualCrossingWeatherProvider : IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiSettings _settings;

    public VisualCrossingWeatherProvider(HttpClient httpClient, WeatherApiSettings settings) {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<WeatherObservation?> GetHistoricalWeatherAsync(double latitude,
        double longitude,
        DateTime timestampUtc,
        CancellationToken cancellationToken = default) {
        var utc = timestampUtc.ToUniversalTime();

        // Request surrounding dates to accommodate the
        // difference between UTC and the location's timezone.
        var startDate = utc.Date.AddDays(-1);
        var endDate = utc.Date.AddDays(1);

        var location = string.Create(CultureInfo.InvariantCulture, $"{latitude:F6},{longitude:F6}");

        var endpoint = _settings.Uri.ToString().TrimEnd('/');

        var requestUri =
            $"{endpoint}/{location}/"
            + $"{startDate:yyyy-MM-dd}/"
            + $"{endDate:yyyy-MM-dd}"
            + "?unitGroup=metric"
            + "&include=hours"
            + "&contentType=json"
            + $"&key={Uri.EscapeDataString(_settings.Key)}";

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VisualCrossingResponse>(cancellationToken);

        if (result?.Days is null)
            return null;

        var targetEpoch = new DateTimeOffset(utc)
            .ToUnixTimeSeconds();

        var observation = result
            .Days
            .Where(day => day.Hours is not null)
            .SelectMany(day => day.Hours!)
            .Where(hour => hour.DatetimeEpoch.HasValue)
            .OrderBy(hour => Math.Abs(hour.DatetimeEpoch!.Value - targetEpoch))
            .FirstOrDefault();

        if (observation?.DatetimeEpoch is null)
            return null;

        return new WeatherObservation(
            observation.Temp,
            observation.FeelsLike,
            observation.Humidity,
            observation.Pressure,
            observation.Conditions,
            observation.Icon,
            DateTimeOffset.FromUnixTimeSeconds(observation.DatetimeEpoch.Value).UtcDateTime);
    }

    private sealed class VisualCrossingResponse
    {
        [JsonPropertyName("days")]
        public List<VisualCrossingDay>? Days { get; set; }
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

        [JsonPropertyName("humidity")]
        public double? Humidity { get; set; }

        [JsonPropertyName("pressure")]
        public double? Pressure { get; set; }

        [JsonPropertyName("conditions")]
        public string? Conditions { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }
}