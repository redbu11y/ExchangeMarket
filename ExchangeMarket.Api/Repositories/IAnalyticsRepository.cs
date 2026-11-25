using ExchangeMarket.Api.Models;
using ExchangeMarket.Api.Models.Entities;

namespace ExchangeMarket.Api.Repositories;

public interface IAnalyticsRepository
{
    Task<Category?> GetCategoryAsync(int categoryId);
    Task<List<Category>> GetCategoriesAsync();
    Task<List<SoldSale>> GetSoldSalesAsync(int categoryId);
    Task<List<Listing>> GetCategoryListingsAsync(int categoryId);
    Task<Listing?> GetListingWithPriceChangesAsync(int listingId);
    
}