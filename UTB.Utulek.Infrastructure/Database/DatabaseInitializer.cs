using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UTB.Utulek.Infrastructure.Identity;

namespace UTB.Utulek.Infrastructure.Database;

public static class DatabaseInitializer
{
    // Při startu aplikace použije všechny čekající migrace a pak naseeduje role a výchozího administrátora.
    // Migrace tak není potřeba pouštět ručně (Update-Database).
    public static async Task InitializeAsync(IServiceProvider services)
    {
        UtulekDbContext dbContext = services.GetRequiredService<UtulekDbContext>();
        await dbContext.Database.MigrateAsync();

        await RoleSeeder.SeedRolesAsync(services);
        await RoleSeeder.SeedAdminAsync(services);
    }
}
