using ExchangeMarket.Api.DTOs.Analytics;
using ExchangeMarket.Api.Exceptions;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;
using ExchangeMarket.Api.Repositories;
using ExchangeMarket.Api.Service.Analytics;

namespace ExchangeMarket.Api.Service;

public class AnalyticsService(IAnalyticsRepository repo) : IAnalyticsService
{
    public async Task<PriceSuggestionDto> GetPriceSuggestionAsync(int categoryId, Condition condition)
    {
        var category = await repo.GetCategoryAsync(categoryId)
                       ?? throw new NotFoundException($"Category with id {categoryId} not found.");

        var sales = await repo.GetSoldSalesAsync(categoryId);
        var suggested = PriceAnalysis.SuggestedPrice(sales, condition);

        return new PriceSuggestionDto(
            category.Id, category.Name, condition, sales.Count,
            HasEnoughData: suggested is not null,
            SuggestedPrice: suggested,
            Message: suggested is null
                ? $"Not enough data: at least {PriceAnalysis.MinSalesForSuggestion} sold listings are needed, this category has {sales.Count}."
                : null);
    }

    public async Task<ListingEstimate> GetListingEstimateAsync(int categoryId, int price, Condition condition)
    {
        var sales = await repo.GetSoldSalesAsync(categoryId);
        return new ListingEstimate(
            PriceAnalysis.SuggestedPrice(sales, condition),
            PriceAnalysis.Assess(price, condition, sales));
    }
    
    
    public async Task<CategoryStatsDto> GetCategoryStatsAsync(int categoryId)
    {
        var category = await repo.GetCategoryAsync(categoryId)
                       ?? throw new NotFoundException($"Category with id {categoryId} not found.");

        return BuildStats(category, await repo.GetCategoryListingsAsync(categoryId));
    }

    public async Task<List<CategoryStatsDto>> GetOverviewAsync()
    {
        var result = new List<CategoryStatsDto>();
        foreach (var category  in await repo.GetCategoriesAsync())
            result.Add(BuildStats(category, await repo.GetCategoryListingsAsync(category.Id)));
        return result;
    }

    public async Task<PriceHistoryDto> GetPriceHistoryAsync(int listingId)
    {
        var listing = await repo.GetListingWithPriceChangesAsync(listingId)
            ?? throw new NotFoundException($"Listing with id {listingId} not found.");

        var changes = listing.PriceChanges
            .OrderBy(c => c.ChangedDate).ThenBy(c => c.Id)
            .Select(c => new PriceChangeDto(c.OldPrice, c.NewPrice, c.ChangedDate))
            .ToList();

        return new PriceHistoryDto(
            listing.Id, listing.Title, PriceAnalysis.OriginalPrice(listing.Price, listing.PriceChanges), listing.Price,
            changes);
    }

    //AI generated
    private static CategoryStatsDto BuildStats(Category category, List<Listing> listings)
    {
        var sold = listings
            .Where(l => l.Status == ListingStatus.Sold && l.SoldPrice != null && l.SoldDate != null)
            .ToList();

        var active = listings.Count(l => l.Status == ListingStatus.Active);
        var removed = listings.Count(l => l.Status == ListingStatus.Removed);
        var total = sold.Count + active + removed;

        var soldPrices = sold.Select(l => (decimal)l.SoldPrice!.Value).ToList();

        var discount = PriceAnalysis.AverageDiscountPercent(
            sold.Select(l => (PriceAnalysis.OriginalPrice(l.Price, l.PriceChanges), l.SoldPrice!.Value)));
        

        return new CategoryStatsDto(
            category.Id,
            category.Name,
            ActiveCount: active,
            SoldCount: sold.Count,
            RemovedCount: removed,
            MeanSoldPrice:   soldPrices.Count > 0 ? Round(Statistics.Mean(soldPrices)) : null,
            MedianSoldPrice: soldPrices.Count > 0 ? Round(Statistics.Median(soldPrices)) : null,
            StdDevSoldPrice: soldPrices.Count > 1 ? Round(Statistics.SampleStandardDeviation(soldPrices)) : null,
            AverageDaysToSell: sold.Count > 0
                ? Round((decimal)sold.Average(l => (l.SoldDate!.Value - l.CreatedDate).TotalDays))
                : null,
            AverageDiscountPercent: discount is null ? null : Round(discount.Value),
            SalesRatio: total > 0 ? Math.Round((decimal)sold.Count / total, 4) : 0m);
    }
    private static decimal Round(decimal value) => Math.Round(value, 2);
}