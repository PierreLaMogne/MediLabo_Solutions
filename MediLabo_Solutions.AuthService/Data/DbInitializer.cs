using Microsoft.AspNetCore.Identity;

namespace MediLabo_Solutions.AuthService.Data
{
    public class DbInitializer
    {
        /// <summary>
        /// Mise en place d'un utilisateur par défaut avec le rôle "Praticien" si aucun utilisateur n'existe dans la base de données.
        /// </summary>
        /// <param name="serviceProvider">Le fournisseur de services pour accéder aux gestionnaires d'utilisateurs et de rôles</param>
        /// <returns></returns>
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
            var adminUsername = configuration["AdminCredentials:Username"] ?? "MediLabo_admin";
            var adminPassword = configuration["AdminCredentials:Password"] ?? "Medilabo2026!";

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
