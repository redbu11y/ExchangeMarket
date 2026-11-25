using ExchangeMarket.Api.DTOs.Listings;
using ExchangeMarket.Api.Models.Entities;

namespace ExchangeMarket.Api.Service;

public interface IFavoriteService
{
    Task<List<ListingSummaryDto>> GetMyAsync(string userId);
    Task AddAsync(string userId, int listingId);
    Task RemoveAsync(string userId, int listingId);
}