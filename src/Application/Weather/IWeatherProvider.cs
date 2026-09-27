namespace Heracles.Application.Weather;

public interface IWeatherProvider
{
    Task<WeatherObservation?> GetHistoricalWeatherAsync(
        double latitude,
        double longitude,
        DateTime timestampUtc,
        CancellationToken cancellationToken = default);
}

public sealed record WeatherObservation(
    double? Temperature,
    double? FeelsLike,
    double? Humidity,
    double? Pressure,
    string? Conditions,
    string? Icon,
    DateTime ObservationTimeUtc);

