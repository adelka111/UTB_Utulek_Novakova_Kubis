using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace UTB.Utulek.Infrastructure.Identity;

public static class RoleSeeder
{
    private const string AdminEmail = "admin@utulek.cz";
    private const string AdminFullName = "Administrátor";
    private const string AdminPassword = "Admin123!";

    // Vytvoří role Admin, Manager a Zakaznik, pokud v databázi ještě nejsou.
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (string role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    // Vytvoří výchozího administrátora, pokud uživatel s jeho e-mailem ještě neexistuje.
    // Existujícího uživatele (ani jeho heslo či role) nemění.
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByEmailAsync(AdminEmail) != null)
        {
            return;
        }

        // UserName = e-mail, protože přihlášení (PasswordSignInAsync) hledá uživatele podle UserName.
        ApplicationUser admin = new ApplicationUser
        {
            UserName = AdminEmail,
            Email = AdminEmail,
            EmailConfirmed = true,
            FullName = AdminFullName
        };

        IdentityResult result = await userManager.CreateAsync(admin, AdminPassword);
        if (result.Succeeded)
        {
            result = await userManager.AddToRoleAsync(admin, Roles.Admin);
        }

        if (!result.Succeeded)
        {
            string errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Nepodařilo se vytvořit výchozího administrátora: {errors}");
        }
    }
}
