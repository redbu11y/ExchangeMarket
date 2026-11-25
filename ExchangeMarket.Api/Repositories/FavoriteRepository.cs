using ExchangeMarket.Api.Data;
using ExchangeMarket.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeMarket.Api.Repositories;

public class FavoriteRepository(AppDbContext db) : IFavoriteRepository
{
    public Task<List<Favorite>> GetByUserAsync(string userId) =>
        db.Favorites
            .AsNoTracking()
            .Include(f => f.Listing)
                .ThenInclude(l => l.Category)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.Id)
            .ToListAsync();

    public Task<Favorite?> GetAsync(string userId, int listingId) =>
        db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId);

    public Task<bool> ListingExistAsync(int listingId) =>
        db.Listings.AnyAsync(l => l.Id == listingId);
    
    public void Add(Favorite favorite) => db.Favorites.Add(favorite);
    public void Remove(Favorite favorite) => db.Favorites.Update(favorite);
    public Task SaveChangesAsync() => db.SaveChangesAsync();
}