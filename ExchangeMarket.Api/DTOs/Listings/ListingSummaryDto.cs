using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.DTOs.Listings;

public record ListingSummaryDto(
    int Id,
    string Title,
    int Price,
    Condition Condition,
    ListingStatus Status,
    string? ImageUrl,
    int CategoryId,
    string CategoryName,
    DateTime CreatedDate);
    