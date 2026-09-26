using Heracles.Application.Activities;
using Heracles.Application.Import;
using Heracles.Application.Points;
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
        }
    }
}
