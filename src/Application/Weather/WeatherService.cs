using Heracles.Application.Data;
using Microsoft.Extensions.Logging;

namespace Heracles.Application.Weather;

public interface IWeatherService
{
    Task<ActivityWeather?> GetActivityWeatherAsync(Track track, CancellationToken cancellationToken = default);
}

public sealed class WeatherService : IWeatherService
{
    private readonly IWeatherRepository _weatherRepository;
    private readonly IWeatherProvider _weatherProvider;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(IWeatherRepository weatherWeatherRepository, IWeatherProvider weatherWeatherProvider, ILogger<WeatherService> logger)
    {
        _weatherRepository = weatherWeatherRepository;
        _weatherProvider = weatherWeatherProvider;
        _logger = logger;
    }

    public async Task<ActivityWeather?> GetActivityWeatherAsync(Track track, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(track);

        var existing = await _weatherRepository.GetAsync(track.Id, cancellationToken);

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
        
        var midPointTime = firstPoint.Time.Add(track.Duration / 2);
        
        try
        {
            var observation =
                await _weatherProvider.GetHistoricalWeatherAsync(
                    firstPoint.Latitude,
                    firstPoint.Longitude,
                    midPointTime,
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

            await _weatherRepository.SaveAsync(weather, cancellationToken);

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