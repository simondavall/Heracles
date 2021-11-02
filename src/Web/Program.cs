using System;
using System.Threading.Tasks;
using Heracles.Application.GpxTrackAggregate;
using Heracles.Domain.Interfaces;
using Heracles.Infrastructure.Data;
using Heracles.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Heracles.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var gpxDbContext = services.GetRequiredService<GpxDbContext>();
                    if (gpxDbContext.Database.IsSqlServer())
                    {
                        await gpxDbContext.Database.MigrateAsync();
                    }

                    await Infrastructure.Data.GpxDbContextSeed.SeedAsync(gpxDbContext);

                    var appIdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();
                    if (appIdentityDbContext.Database.IsSqlServer())
                    {
                        await appIdentityDbContext.Database.MigrateAsync();
                    }

                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await Infrastructure.Identity.AppIdentityDbContextSeed.SeedAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                    throw;
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
