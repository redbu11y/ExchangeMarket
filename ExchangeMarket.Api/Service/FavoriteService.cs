using ExchangeMarket.Api.DTOs.Listings;
using ExchangeMarket.Api.Exceptions;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Repositories;

namespace ExchangeMarket.Api.Service;

public class FavoriteService(IFavoriteRepository repo) : IFavoriteService
{
    public async Task<List<ListingSummaryDto>> GetMyAsync(string userId) =>
        (await repo.GetByUserAsync(userId)).Select(f=>f.Listing.ToSummaryDto()).ToList();

    public async Task AddAsync(string userId, int listingId)
    {
        if (!await repo.ListingExistAsync(listingId))
            throw new NotFoundException($"Listing with id {listingId} does not exist.");

        if (await repo.GetAsync(userId, listingId) is not null)
        {
            throw new ConflictException($"Listing with id {listingId} is already in favorites.");
        }
        
        repo.Add(new Favorite{UserId = userId, ListingId = listingId});
        await repo.SaveChangesAsync();
    }

    public async Task RemoveAsync(string userId, int listingId)
    {
        var favorite = await repo.GetAsync(userId, listingId)
            ?? throw new NotFoundException($"Favorite with id {listingId} does not exist.");
        
        repo.Remove(favorite);
        await repo.SaveChangesAsync();
    }
}