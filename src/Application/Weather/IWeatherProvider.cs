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
    string? Conditions);

