using Heracles.Application.Activities;
using Heracles.Application.Configuration;
using Heracles.Application.Import;
using Heracles.Application.Pace;
using Heracles.Application.Speed;
using Heracles.Application.TrackPoints;
using Heracles.Application.Weather;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, HeraclesSettings settings)
        {
            services.AddScoped<IImportService, ImportService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddScoped<ITrackPointDataService, TrackPointDataService>();
            services.AddScoped<IPaceService, PaceService>();
            services.AddScoped<ISpeedService, SpeedService>();
            

            services.AddSingleton(settings.Elevation);
            services.AddSingleton(settings.Import);
            services.AddSingleton(settings.Mapbox);
            services.AddSingleton(settings.Pace);
            services.AddSingleton(settings.Speed);
            services.AddSingleton(settings.WeatherApi);
        }
    }
}
