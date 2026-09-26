using Heracles.Application.Configuration;
using Heracles.Application.Data;
using Heracles.Application.Import;
using Heracles.Infrastructure.Data;
using Heracles.Infrastructure.Gpx;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Heracles.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, HeraclesSettings settings) {
            var connectionString = new SqliteConnectionStringBuilder {
                DataSource = settings.DatabaseSettings.DatabasePath, 
                Mode = SqliteOpenMode.ReadWriteCreate, 
                ForeignKeys = true
            }.ToString();

            services.AddDbContextFactory<HeraclesDbContext>(options => options.UseSqlite(connectionString));

            services.AddScoped<ITrackRepository, TrackRepository>();
            services.AddTransient<IGpxService, GpxService>();
        }

        public static IApplicationBuilder UseMigrationsEndPoint(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment())
                app.UseMigrationsEndPoint();

            return app;
        }
    }
}