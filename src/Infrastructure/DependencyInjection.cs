using Heracles.Application.Interfaces;
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
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContextFactory<GpxDbContext>(options => {
                if (configuration.GetValue<bool>("UseInMemoryDatabase"))
                    options.UseInMemoryDatabase("HeraclesDb");
                else
                    options.UseSqlServer(configuration.GetConnectionString("HeraclesDb"));
            });

            services.AddScoped<ITrackRepository, TrackRepository>();
            services.AddTransient<IGpxService, GpxService>();
        }

        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<AppIdentityDbContext>(options => {
                if (configuration.GetValue<bool>("UseInMemoryDatabase"))
                    options.UseInMemoryDatabase("HeraclesAuthDb");
                else
                    options.UseSqlServer(configuration.GetConnectionString("HeraclesAuthDb"));
            });

            services
                .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();

            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        public static IApplicationBuilder AddInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env) {
            app = UseMigrationsEndPoint(app, env);
            return app;
        }

        internal static IApplicationBuilder UseMigrationsEndPoint(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseMigrationsEndPoint();
            }

            return app;
        }
    }
}