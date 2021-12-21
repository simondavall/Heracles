using Heracles.Application.Interfaces;
using Heracles.Application.Services;
using Heracles.Application.Services.Import;
using Heracles.Application.Services.Points;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IImportService, ImportService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IPointService, PointService>();
            services.AddSingleton<IImportProgressService, ImportProgressService>();
        }
    }
}
