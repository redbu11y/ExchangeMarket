using ExchangeMarket.Api.Models.Entities;

namespace ExchangeMarket.Api.Repositories;

public interface IFavoriteRepository
{
    Task<List<Favorite>> GetByUserAsync(string userId);
    Task<Favorite?> GetAsync(string userId, int listingId);
    Task<bool> ListingExistAsync(int listingId);
    void Add(Favorite favorite);
    void Remove(Favorite favorite);
    Task SaveChangesAsync();
    
}