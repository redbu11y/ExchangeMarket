using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.DTOs.Offers;

public record OfferDto(
    int Id,
    int ListingId,
    string ListingTitle,
    string BuyerId,
    String BuyerName,
    int Amount,
    OfferStatus Status,
    DateTime CreatedDate);