using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.DTOs.Listings;

public record ListingDetailDto(
    int Id,
    string Title,
    string? Description,
    int Price,
    Condition Condition,
    ListingStatus Status,
    string? ImageUrl,
    DateTime CreatedDate,
    DateTime? SoldDate,
    int? SoldPrice,
    int CategoryId,
    string CategoryName,
    string SellerId,
    string SellerName,
    int OfferCount,
    int? SuggestedPrice,
    string? PriceAssessment);