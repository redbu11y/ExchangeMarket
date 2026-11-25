namespace ExchangeMarket.Api.DTOs.Analytics;

public record CategoryStatsDto (
    int CategoryId,
    string CategoryName,
    int ActiveCount,
    int SoldCount,
    int RemovedCount,
    decimal? MeanSoldPrice,
    decimal? MedianSoldPrice,
    decimal? AverageDaysToSell,
    decimal? AverageDiscountPercent,
    decimal SalesRatio
    );