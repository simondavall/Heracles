namespace Heracles.Application.Weather;

public interface IWeatherProvider
{
    Task<WeatherObservation?> GetHistoricalWeatherAsync(
        double latitude,
        double longitude,
        DateTime datetime,
        CancellationToken cancellationToken = default);
}

public sealed record WeatherObservation(
    double? Temperature,
    double? FeelsLike,
    string? Conditions,
    WeatherCode? WeatherCode,
    DateTime ObservationTimeUtc);

