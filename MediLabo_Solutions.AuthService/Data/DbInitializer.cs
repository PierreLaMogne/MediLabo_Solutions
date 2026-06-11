using Microsoft.AspNetCore.Identity;

namespace MediLabo_Solutions.AuthService.Data
{
    public class DbInitializer
    {
        public static async Task InitializeUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // Création du rôle
            if(!await roleManager.RoleExistsAsync("Praticien"))
            {
                await roleManager.CreateAsync(new IdentityRole("Praticien"));
            }

            // Création de l'utilisateur
            var adminUsername = configuration["AdminUser:Username"] ?? "MediLabo_admin";
            var adminPassword = configuration["AdminUser:Password"] ?? "Medilabo2026!";

            if (await userManager.FindByNameAsync(adminUsername) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminUsername,
                    Email = $"{adminUsername}@medilabo.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Praticien");
                }
            }
        }
    }
}
