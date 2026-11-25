using ExchangeMarket.Api.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExchangeMarket.Api.Data;

public class DbSeedData
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService < UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        await db.Database.MigrateAsync();
        
        foreach (var role in new[] { "AdminRole", "UserRole" })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        
        //users
        await SeedUserAsync(userManager, "admin@brukt.no", "Admin", "Admin123!", AdminRole);
        var ola = await SeedUserAsync(userManager, "ola@brukt.no",  "Ola Bruker", "bruker123!", UserRole);
        var kari = await SeedUserAsync(userManager, "kari@brukt.no",  "Kari Bruker", "bruker1234!", UserRole);
        var per = await SeedUserAsync(userManager, "per@brukt.no",  "Per Bruker", "bruker12345!", UserRole);

        //check if data is seeded
        if (await db.Categories.AnyAsync()) return;
        
    }

    private static async Task<ApplicationUser> SeedUserAsync(
        UserManager<ApplicationUser> userManager, string email, string fullName, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = emaill, Email = email, Fullname = fullName, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new InvalidOperationException("seeding failed");
        }
        if (!await userManager.IsInRoleAsync(user, role)) await  userManager.AddToRoleAsync(user, role);
        
        return user;
    }
    
}