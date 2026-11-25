using ExchangeMarket.Api.DTOs.Offers;
using ExchangeMarket.Api.Models.Entities;

namespace ExchangeMarket.Api.Service;

public interface IOfferService
{
    Task<List<OfferDto>> GetForListingAsync(int listingId, string userId);
    Task<List<OfferDto>> GetMyAsync(string userId);
    Task<OfferDto> PlaceAsync(int listingId, CreateOfferDto dto, string buyerId);
    Task<OfferDto> AcceptAsync(int offerId, string userId);
    Task<OfferDto> RejectAsync(int offerId, string userId);
    Task<OfferDto> WithdrawAsync(int offerId, string userId);
}