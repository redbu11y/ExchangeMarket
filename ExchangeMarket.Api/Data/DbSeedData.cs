using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;
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
        
        foreach (var role in new[] { AdminRole, UserRole })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        
        //users
        await SeedUserAsync(userManager, "admin@brukt.no", "Admin", "Admin123!", AdminRole);
        var ola = await SeedUserAsync(userManager, "ola@brukt.no",  "Ola Bruker", "Bruker123!", UserRole);
        var kari = await SeedUserAsync(userManager, "kari@brukt.no",  "Kari Bruker", "Bruker1234!", UserRole);
        var per = await SeedUserAsync(userManager, "per@brukt.no",  "Per Bruker", "Bruker12345!", UserRole);

        //check if data is seeded
        if (await db.Categories.AnyAsync()) return;
        
        SeedMarketData(db, ola, kari, per);
        await db.SaveChangesAsync();
        
    }

    private static async Task<ApplicationUser> SeedUserAsync(
        UserManager<ApplicationUser> userManager, string email, string fullName, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new InvalidOperationException($"Seeding {email} failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        if (!await userManager.IsInRoleAsync(user, role)) await  userManager.AddToRoleAsync(user, role);
        
        return user;
    }

    // AI generated seed data
    private static void SeedMarketData(AppDbContext db, ApplicationUser ola, ApplicationUser kari, ApplicationUser per)
    {
        // ---------- Helpers ----------
        static DateTime D(int y, int m, int d) => new(y, m, d, 12, 0, 0, DateTimeKind.Utc);

        Listing Sold(Category cat, string title, Condition cond, int price, int soldPrice,
                     ApplicationUser seller, ApplicationUser buyer, DateTime created, int daysToSell)
        {
            var soldDate = created.AddDays(daysToSell);
            var listing = new Listing
            {
                Title = title,
                Description = $"{title} in {cond} condition.",
                Category = cat,
                SellerId = seller.Id,
                Condition = cond,
                Price = price,
                Status = ListingStatus.Sold,
                CreatedDate = created,
                SoldDate = soldDate,
                SoldPrice = soldPrice
            };
            listing.Offers.Add(new Offer
            {
                BuyerId = buyer.Id, Amount = soldPrice,
                Status = OfferStatus.Accepted, CreatedDate = soldDate.AddDays(-1)
            });
            db.Listings.Add(listing);
            return listing;
        }

        Listing Open(Category cat, string title, Condition cond, int price, ApplicationUser seller,
                     DateTime created, ListingStatus status = ListingStatus.Active)
        {
            var listing = new Listing
            {
                Title = title,
                Description = $"{title} in {cond} condition.",
                Category = cat,
                SellerId = seller.Id,
                Condition = cond,
                Price = price,
                Status = status,
                CreatedDate = created
            };
            db.Listings.Add(listing);
            return listing;
        }

        static void Offer(Listing l, ApplicationUser buyer, int amount, OfferStatus status, DateTime date) =>
            l.Offers.Add(new Offer { BuyerId = buyer.Id, Amount = amount, Status = status, CreatedDate = date });

        static void PriceChange(Listing l, int oldPrice, int newPrice, DateTime date) =>
            l.PriceChanges.Add(new PriceChange { OldPrice = oldPrice, NewPrice = newPrice, ChangedDate = date });

        // ---------- Categories ----------
        var electronics = new Category { Name = "Electronics" };
        var bicycles    = new Category { Name = "Bicycles" };
        var furniture   = new Category { Name = "Furniture" };
        var outdoor     = new Category { Name = "Outdoor Gear" };
        db.Categories.AddRange(electronics, bicycles, furniture, outdoor);

        // ---------- Electronics: 8 sold ----------
        // Normalized: 4000, 4000, 3000, 4000, 5000, 3000, 6000, 2000
        var e1 = Sold(electronics, "Noise-cancelling headphones", Condition.New,     4200, 4000, ola,  kari, D(2026, 1, 10), 12);
        Sold(electronics, "Smartphone 128 GB",                    Condition.LikeNew, 3600, 3400, kari, per,  D(2026, 2, 3),  6);
        Sold(electronics, "Tablet 10 inch",                       Condition.Good,    2400, 2100, per,  ola,  D(2026, 2, 20), 15);
        Sold(electronics, "Gaming console",                       Condition.Used,    2500, 2200, ola,  per,  D(2026, 3, 8),  9);
        Sold(electronics, "Laptop 14 inch",                       Condition.Good,    3900, 3500, kari, ola,  D(2026, 4, 1),  20);
        Sold(electronics, "Smartwatch",                           Condition.Used,    1800, 1650, per,  kari, D(2026, 5, 12), 4);
        Sold(electronics, "Mirrorless camera",                    Condition.LikeNew, 5500, 5100, ola,  kari, D(2026, 6, 2),  11);
        Sold(electronics, "Bluetooth speaker (broken button)",    Condition.Defect,   800,  600, kari, per,  D(2026, 7, 15), 30);

        PriceChange(e1, 5000, 4500, D(2026, 1, 15));   // original price = 5000
        PriceChange(e1, 4500, 4200, D(2026, 1, 18));
        Offer(e1, per, 3000, OfferStatus.Rejected, D(2026, 1, 16));

        // Electronics: active + removed
        var ea1 = Open(electronics, "Wireless earbuds",     Condition.Good,    2800, per,  D(2026, 10, 5));   // Normal
        Open(electronics, "Old e-reader",                   Condition.Used,     500, ola,  D(2026, 10, 20));  // Low
        var ea3 = Open(electronics, "4K monitor 27 inch",   Condition.LikeNew, 3800, kari, D(2026, 11, 1));   // Normal
        var ea4 = Open(electronics, "Drone with camera",    Condition.Good,    4900, ola,  D(2026, 11, 10));  // High
        Open(electronics, "Wi-Fi router",                   Condition.Used,     400, per,  D(2026, 3, 1), ListingStatus.Removed);

        PriceChange(ea1, 3200, 2800, D(2026, 10, 12));
        Offer(ea1, ola,  2200, OfferStatus.Pending,   D(2026, 10, 14));
        Offer(ea1, kari, 2400, OfferStatus.Pending,   D(2026, 10, 15));
        Offer(ea3, ola,  3000, OfferStatus.Withdrawn, D(2026, 11, 3));

        // ---------- Bicycles: 7 sold ----------
        // Normalized: 5000, 5000, 6000, 7000, 4000, 6000, 4000
        Sold(bicycles, "Hybrid bike",               Condition.Good,    3800, 3500, kari, ola,  D(2026, 3, 15), 14);
        Sold(bicycles, "City bike",                 Condition.Used,    3000, 2750, per,  kari, D(2026, 4, 2),  8);
        var b3 = Sold(bicycles, "Road bike",        Condition.LikeNew, 5500, 5100, ola,  per,  D(2026, 4, 20), 10);
        var b4 = Sold(bicycles, "Mountain bike",    Condition.New,     7500, 7000, kari, per,  D(2026, 5, 5),  18);
        Sold(bicycles, "Kids' bike 20 inch",        Condition.Good,    3000, 2800, per,  ola,  D(2026, 5, 25), 5);
        Sold(bicycles, "Gravel bike",               Condition.Used,    3500, 3300, ola,  kari, D(2026, 6, 14), 12);
        Sold(bicycles, "Racing bike (bent wheel)",  Condition.Defect,  1500, 1200, per,  ola,  D(2026, 7, 1),  25);

        PriceChange(b4, 8000, 7500, D(2026, 5, 12));
        Offer(b3, kari, 4000, OfferStatus.Rejected, D(2026, 4, 25));

        // Bicycles: active + removed
        var ba1 = Open(bicycles, "Vintage bike",    Condition.Used,    45000, ola,  D(2026, 10, 1));   // inflated → High
        var ba2 = Open(bicycles, "Commuter bike",   Condition.Good,     3400, kari, D(2026, 10, 18));
        Open(bicycles, "BMX",                       Condition.LikeNew,  4000, per,  D(2026, 11, 3));
        Open(bicycles, "Tandem bike",               Condition.Good,     5200, ola,  D(2026, 11, 12));
        Open(bicycles, "Folding bike",              Condition.Used,     2000, kari, D(2026, 6, 1), ListingStatus.Removed);

        Offer(ba2, per, 2900, OfferStatus.Pending, D(2026, 10, 22));

        // ---------- Furniture: 6 sold ----------
        // Normalized: 2000, 1000, 3000, 1500, 2500, 2000
        Sold(furniture, "Oak dining table",         Condition.Good,    1600, 1400, ola,  per,  D(2026, 2, 10), 21);
        Sold(furniture, "Office chair",             Condition.Used,     700,  550, kari, ola,  D(2026, 3, 3),  7);
        var f3 = Sold(furniture, "Leather sofa",    Condition.LikeNew, 2800, 2550, per,  kari, D(2026, 4, 10), 16);
        Sold(furniture, "Bookshelf",                Condition.New,     1500, 1500, ola,  kari, D(2026, 5, 18), 3);
        Sold(furniture, "Wardrobe",                 Condition.Good,    1900, 1750, kari, per,  D(2026, 6, 20), 13);
        Sold(furniture, "Coffee table",             Condition.Used,    1200, 1100, per,  ola,  D(2026, 8, 5),  9);

        PriceChange(f3, 3000, 2800, D(2026, 4, 20));
        Offer(f3, ola, 2000, OfferStatus.Rejected, D(2026, 4, 22));

        // Furniture: active
        var fa1 = Open(furniture, "Desk",                     Condition.Good,    1500, ola,  D(2026, 10, 8));
        var fa2 = Open(furniture, "Armchair",                 Condition.LikeNew, 2000, kari, D(2026, 10, 25));
        Open(furniture, "Bed frame 180 cm",                   Condition.Used,    1200, per,  D(2026, 11, 2));
        Open(furniture, "TV bench",                           Condition.Good,     900, ola,  D(2026, 11, 15));
        Open(furniture, "Dining chairs (set of 4)",           Condition.New,     2500, kari, D(2026, 11, 18));

        Offer(fa1, kari, 1200, OfferStatus.Pending,  D(2026, 10, 12));
        Offer(fa2, per,  1100, OfferStatus.Rejected, D(2026, 10, 28));

        // ---------- Outdoor Gear: only 2 sold → "insufficient data" ----------
        Sold(outdoor, "Tent 2-person",              Condition.Good, 1500, 1400, kari, per, D(2026, 5, 1),  10);
        Sold(outdoor, "Hiking backpack 60 L",       Condition.Used,  950,  880, per,  ola, D(2026, 7, 20), 6);

        Open(outdoor, "Sleeping bag",               Condition.LikeNew, 1200, ola,  D(2026, 10, 10));
        var oa2 = Open(outdoor, "Cross-country skis", Condition.Good,  1800, kari, D(2026, 10, 28));
        Open(outdoor, "Camping stove",              Condition.Used,     400, per,  D(2026, 11, 5));
        var oa4 = Open(outdoor, "Kayak",            Condition.Good,    6000, ola,  D(2026, 11, 14));

        Offer(oa4, per, 4500, OfferStatus.Pending, D(2026, 11, 16));

        // ---------- Favorites (never on own listings) ----------
        db.Favorites.AddRange(
            new Favorite { Listing = ba2, UserId = ola.Id },
            new Favorite { Listing = oa2, UserId = ola.Id },
            new Favorite { Listing = ea1, UserId = kari.Id },
            new Favorite { Listing = oa4, UserId = kari.Id },
            new Favorite { Listing = ba1, UserId = per.Id },
            new Favorite { Listing = fa2, UserId = per.Id },
            new Favorite { Listing = ea4, UserId = per.Id });
    }
}