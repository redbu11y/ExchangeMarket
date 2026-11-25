using Microsoft.EntityFrameworkCore;

namespace ExchangeMarket.Api.Data;

public class DbSeedData
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }
}