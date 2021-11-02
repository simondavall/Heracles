using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var appIdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();
            await RunMigrationsIfNeeded(appIdentityDbContext);
            await SeedInitialData(services);
        }

        private static async Task SeedInitialData(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.Roles.AnyAsync())
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                const string defaultPassword = "Password123..";
                await roleManager.CreateAsync(new IdentityRole("Administrators"));
                await roleManager.CreateAsync(new IdentityRole("HeraclesUser"));

                await AddNewUser(userManager, "admin@email.com", defaultPassword, "Administrators");
                await AddNewUser(userManager, "user@email.com", defaultPassword, "HeraclesUser");
            }
        }

        private static async Task AddNewUser(UserManager<ApplicationUser> userManager, string username, string password, string addToRole)
        {
            var appUser = new ApplicationUser {UserName = username, Email = username };
            await userManager.CreateAsync(appUser, password);
            var adminToken = await userManager.GenerateEmailConfirmationTokenAsync(appUser);
            await userManager.ConfirmEmailAsync(appUser, adminToken);
            await userManager.AddToRoleAsync(appUser, addToRole);
        }

        private static async Task RunMigrationsIfNeeded(AppIdentityDbContext appIdentityDbContext)
        {
            if (appIdentityDbContext.Database.IsSqlServer())
            {
                await appIdentityDbContext.Database.MigrateAsync();
            }
        }
    }
}
