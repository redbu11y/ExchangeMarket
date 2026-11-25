using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.DTOs.Offers;

public record OfferDto(
    int Id,
    int ListingId,
    string ListingTitle,
    string BuyerId,
    string BuyerName,
    int Amount,
    OfferStatus Status,
    DateTime CreatedDate);