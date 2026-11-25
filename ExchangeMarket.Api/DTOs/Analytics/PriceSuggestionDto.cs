using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.DTOs.Analytics;

public record  PriceSuggestionDto(
    int CategoryId,
    string CategoryName,
    Condition Condition,
    int SalesCount,
    bool HasEnoughData,
    int? SuggestedPrice,
    string? Message);