using Heracles.Application.Weather;
using Heracles.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Infrastructure.Weather;

public sealed class WeatherRepository : IWeatherRepository
{
    private readonly IDbContextFactory<HeraclesDbContext> _contextFactory;

    public WeatherRepository(IDbContextFactory<HeraclesDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<ActivityWeather?> GetAsync(Guid trackId, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.ActivityWeather
            .AsNoTracking()
            .FirstOrDefaultAsync(weather => weather.TrackId == trackId, cancellationToken);
    }

    public async Task SaveAsync(ActivityWeather weather, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        context.ActivityWeather.Add(weather);

        await context.SaveChangesAsync(cancellationToken);
    }
}