using ExchangeMarket.Api.DTOs.Listings;
using ExchangeMarket.Api.Models.Entities;

namespace ExchangeMarket.Api.Service;

public interface IListingService
{
    Task<List<ListingSummaryDto>> SearchAsync(ListingQueryDto query);
    Task<ListingDetailDto> GetByIdAsync(int id);
    Task<List<ListingSummaryDto>> GetMyAsync(string userId);
    Task<ListingDetailDto> CreateAsync(ListingRequestDto listingRequestDto, string userId);
    Task<ListingDetailDto> UpdateAsync(int id, ListingRequestDto dto, string userId, bool isAdmin);
    Task RemoveAsync(int id, string userId, bool isAdmin);
}