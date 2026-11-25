using ExchangeMarket.Api.DTOs.Analytics;
using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.Service;

public record ListingEstimate(int? SuggestedPrice, PriceAssessment Assessment);

public interface IAnalyticsService
{
    Task<PriceSuggestionDto> GetPriceSuggestionAsync(int categoryId, Condition condition);
    Task<CategoryStatsDto> GetCategoryStatsAsync(int categoryId);
    Task<List<CategoryStatsDto>> GetOverviewAsync();
    Task<PriceHistoryDto> GetPriceHistoryAsync(int listingId);
    Task<ListingEstimate> GetListingEstimateAsync(int categoryId, int price, Condition condition);
    

}