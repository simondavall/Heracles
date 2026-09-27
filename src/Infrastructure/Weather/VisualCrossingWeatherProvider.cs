using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Heracles.Application.Configuration;
using Heracles.Application.Weather;
using Microsoft.Extensions.Logging;

namespace Heracles.Infrastructure.Weather;

public sealed class VisualCrossingWeatherProvider : IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiSettings _settings;
    private readonly ILogger<VisualCrossingWeatherProvider> _logger;

    public VisualCrossingWeatherProvider(HttpClient httpClient, WeatherApiSettings settings, ILogger<VisualCrossingWeatherProvider> logger) {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task<WeatherObservation?> GetHistoricalWeatherAsync(double latitude,
        double longitude,
        DateTime timestampUtc,
        CancellationToken cancellationToken = default) {
        var utc = timestampUtc.Kind switch {
            DateTimeKind.Utc => timestampUtc,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(
                timestampUtc,
                DateTimeKind.Utc),
            _ => timestampUtc.ToUniversalTime()
        };

        var location = string.Create(CultureInfo.InvariantCulture, $"{latitude:F6},{longitude:F6}");
        var dateTime = utc.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
        var endpoint = _settings.Uri.ToString().TrimEnd('/');

        var requestUriWithoutKey =
            $"{endpoint}/{location}/{dateTime}"
            + "?unitGroup=metric"
            + "&timezone=Z"
            + "&include=current"
            + "&contentType=json";
        
        var requestUri = requestUriWithoutKey + $"&key={Uri.EscapeDataString(_settings.Key)}";

        _logger.LogInformation("Calling WeatherApi with {WeatherApiRequest}", requestUriWithoutKey);

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<VisualCrossingResponse>(cancellationToken);

        _logger.LogInformation("Response from WeatherApi: {WeatherApiResponse}", result);
        
        var queryCost = result?.QueryCost;
        var observation = result?.CurrentConditions;

        if (observation?.DatetimeEpoch is null)
            return null;

        _logger.LogInformation("Query cost of api call: {QueryCost}", queryCost);
        
        return new WeatherObservation(
            observation.Temp,
            observation.FeelsLike,
            observation.Humidity,
            observation.Pressure,
            observation.Conditions,
            observation.Icon,
            DateTimeOffset.FromUnixTimeSeconds(observation.DatetimeEpoch.Value).UtcDateTime);
    }

    private sealed record VisualCrossingResponse
    {
        [JsonPropertyName("queryCost")]
        public int? QueryCost { get; set; }
        
        [JsonPropertyName("currentConditions")]
        public VisualCrossingCurrentConditions? CurrentConditions { get; set; }
    }

    private sealed record VisualCrossingCurrentConditions
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