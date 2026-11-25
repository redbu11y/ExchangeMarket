using ExchangeMarket.Api.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExchangeMarket.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<PriceChange> PriceChanges => Set<PriceChange>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Category
        builder.Entity<Category>(e =>
        {
            e.HasIndex(c => c.Name).IsUnique();
        });

        // Listing
        builder.Entity<Listing>(e =>
        {
            e.Property(l => l.Condition).HasConversion<string>().HasMaxLength(20);
            e.Property(l => l.Status).HasConversion<string>().HasMaxLength(20);
            e.ToTable(t => t.HasCheckConstraint("CK_Listing_Price", "Price > 0"));
            
            e.HasOne(l => l.Category)
                .WithMany(c => c.Listings)
                .HasForeignKey(l => l.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
            e.HasOne(l => l.Seller)
                .WithMany(s => s.Listings)
                .HasForeignKey(l => l.SellerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OFFER
        builder.Entity<Offer>(e =>
        {
            e.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            
            e.HasOne(o => o.Listing)
                .WithMany(l => l.Offers)
                .HasForeignKey(o => o.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
            
            e.HasOne(o => o.Buyer)
                .WithMany(b => b.Offers)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        //Price Change
        builder.Entity<PriceChange>(e =>
        {
            e.HasOne(p => p.Listing)
                .WithMany(l => l.PriceChanges)
                .HasForeignKey(p => p.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        //Favorite
        builder.Entity<Favorite>(e =>
        {
            e.HasOne(f => f.Listing)
                .WithMany(l => l.Favorites)
                .HasForeignKey(f => f.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
            
            e.HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }
}