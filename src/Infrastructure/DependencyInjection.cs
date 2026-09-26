using Heracles.Application.Interfaces;
using Heracles.Infrastructure.Data;
using Heracles.Infrastructure.Gpx;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Heracles.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContextFactory<HeraclesDbContext>(options => {
                if (configuration.GetValue<bool>("UseInMemoryDatabase"))
                    options.UseInMemoryDatabase("HeraclesDb");
                else
                    options.UseSqlServer(configuration.GetConnectionString("HeraclesDb"));
            });

            services.AddScoped<ITrackRepository, TrackRepository>();
            services.AddTransient<IGpxService, GpxService>();
        }

        public static IApplicationBuilder UseMigrationsEndPoint(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseMigrationsEndPoint();
            }

            return app;
        }
    }
}