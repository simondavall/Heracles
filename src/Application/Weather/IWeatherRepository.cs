using Heracles.Application.Data;

namespace Heracles.Application.Weather;

public interface IWeatherRepository
{
    Task<ActivityWeather?> GetAsync(Guid trackId, CancellationToken cancellationToken = default);
    Task SaveAsync(ActivityWeather weather, CancellationToken cancellationToken = default);
}