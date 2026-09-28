using Heracles.Application.Data;
using Microsoft.Extensions.Logging;

namespace Heracles.Application.Weather;

public interface IWeatherService
{
    Task<ActivityWeather?> GetActivityWeatherAsync(Track track, CancellationToken cancellationToken = default);
}

public sealed class WeatherService : IWeatherService
{
    private readonly IWeatherRepository _repository;
    private readonly IWeatherProvider _provider;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(IWeatherRepository repository, IWeatherProvider provider, ILogger<WeatherService> logger)
    {
        _repository = repository;
        _provider = provider;
        _logger = logger;
    }

    public async Task<ActivityWeather?> GetActivityWeatherAsync(Track track, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(track);

        var existing = await _repository.GetAsync(track.Id, cancellationToken);

        if (existing is not null)
            return existing;
        
        var firstPoint = track.TrackSegments
            .OrderBy(segment => segment.Seq)
            .SelectMany(segment => segment.TrackPoints.OrderBy(point => point.Seq))
            .FirstOrDefault();
        
        if (firstPoint is null)
        {
            _logger.LogWarning("Activity {TrackId} has no GPS points", track.Id);

            return null;
        }

        // The GPX reader normalises recorded timestamps to UTC.
        // SQLite may return the persisted value with Kind=Unspecified.
        var timestampUtc = firstPoint.Time.Kind switch
        {
            DateTimeKind.Utc => firstPoint.Time,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(firstPoint.Time, DateTimeKind.Utc),
            _ => firstPoint.Time.ToUniversalTime()
        };
        
        try
        {
            // Find the weather for the mid-point of the activity
            var observation =
                await _provider.GetHistoricalWeatherAsync(
                    firstPoint.Latitude,
                    firstPoint.Longitude,
                    timestampUtc.Add(track.Duration / 2),
                    cancellationToken);

            if (observation is null)
                return null;

            var weather = new ActivityWeather
            {
                TrackId = track.Id,

                Temperature = observation.Temperature,
                FeelsLike = observation.FeelsLike,
                Conditions = observation.Conditions
            };

            await _repository.SaveAsync(weather, cancellationToken);

            return weather;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Unable to retrieve weather for activity {TrackId}", track.Id);
            return null;
        }
    }
}