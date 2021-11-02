using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Heracles.Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string defaultPassword = "Password123..";
            await roleManager.CreateAsync(new IdentityRole("Administrators"));
            await roleManager.CreateAsync(new IdentityRole("HeraclesUser"));

            const string adminUserName = "admin@email.com";
            var adminUser = new ApplicationUser { UserName = adminUserName, Email = adminUserName };
            await userManager.CreateAsync(adminUser, defaultPassword);
            var adminToken = await userManager.GenerateEmailConfirmationTokenAsync(adminUser);
            await userManager.ConfirmEmailAsync(adminUser, adminToken);

            const string demoUserName = "user@email.com";
            var demoUser = new ApplicationUser { UserName = demoUserName, Email = demoUserName };
            await userManager.CreateAsync(demoUser, defaultPassword);
            var demoUserToken = await userManager.GenerateEmailConfirmationTokenAsync(demoUser);
            await userManager.ConfirmEmailAsync(demoUser, demoUserToken);

            adminUser = await userManager.FindByNameAsync(adminUserName);
            await userManager.AddToRoleAsync(adminUser, "Administrators");

            demoUser = await userManager.FindByNameAsync(demoUserName);
            await userManager.AddToRoleAsync(demoUser, "HeraclesUser");
        }
    }
}
