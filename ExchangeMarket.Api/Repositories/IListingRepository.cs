using ExchangeMarket.Api.DTOs.Listings;
using ExchangeMarket.Api.Models.Entities;

namespace ExchangeMarket.Api.Repositories;

public interface  IListingRepository
{
    Task<List<Listing>> SearchActiveAsync(ListingQueryDto query);
    Task<Listing?> GetByIdAsync(int id);
    Task<Listing?> GetDetailsAsync(int id);
    Task<List<Listing>> GetBySellerAsync(string sellerId);
    Task<bool> CategoryExistsAsync(int categoryId);
    void Add(Listing listing);
    Task SaveChangesAsync();
}