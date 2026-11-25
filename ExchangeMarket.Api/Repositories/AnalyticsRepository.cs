using ExchangeMarket.Api.Data;
using ExchangeMarket.Api.Models;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExchangeMarket.Api.Repositories;

public class AnalyticsRepository(AppDbContext db) : IAnalyticsRepository
{
    public Task<Category?> GetCategoryAsync(int categoryId) =>
        db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId);

    public Task<List<Category>> GetCategoriesAsync() =>
        db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

    public Task<List<SoldSale>> GetSoldSalesAsync(int categoryId) =>
        db.Listings
            .Where(l => l.CategoryId == categoryId &&
                        l.Status == ListingStatus.Sold &&
                        l.SoldPrice != null)
            .Select(l => new SoldSale(l.SoldPrice!.Value, l.Condition))
            .ToListAsync();
    
    
    public Task<List<Listing>> GetCategoryListingsAsync(int categoryId) =>
        db.Listings
            .AsNoTracking()
            .Include(l => l.PriceChanges)
            .Where(l => l.CategoryId == categoryId)
            .ToListAsync();

    public Task<Listing?> GetListingWithPriceChangesAsync(int listingId) =>
        db.Listings
            .AsNoTracking()
            .Include(l => l.PriceChanges)
            .FirstOrDefaultAsync(l => l.Id == listingId);
}