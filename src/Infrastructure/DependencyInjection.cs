using Heracles.Application.Interfaces;
using Heracles.Domain.Interfaces;
using Heracles.Infrastructure.Data;
using Heracles.Infrastructure.Gpx;
using Heracles.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Heracles.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services = AddDatabaseContexts(services, configuration);

            services.AddScoped<ITrackRepository, TrackRepository>();

            services.AddTransient<IGpxService, GpxService>();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();
        }

        internal static IServiceCollection AddDatabaseContexts(IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<GpxDbContext>(options =>
                    options.UseInMemoryDatabase("HeraclesDb"));

                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseInMemoryDatabase("HeraclesAuthDb"));
            }
            else
            {
                services.AddDbContext<GpxDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("HeraclesDb")));

                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("HeraclesAuthDb")));
            }

            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }

        public static IApplicationBuilder AddInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app = UseMigrationsEndPoint(app, env);
            return app;
        }

        internal static IApplicationBuilder UseMigrationsEndPoint(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }

            return app;
        }
    }
}
