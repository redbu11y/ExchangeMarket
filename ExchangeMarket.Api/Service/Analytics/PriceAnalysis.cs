using ExchangeMarket.Api.Models;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.Service.Analytics;

public class PriceAnalysis
{
    public const int MinSalesForSuggestion = 3;
    public const int MinSalesForAssessment  = 5;
    
    public static List<decimal> NormalizedPrices(IEnumerable<SoldSale> sales) =>
        sales.Select(s=>ConditionFactors.Normalize(s.SoldPrice, s.Condition)).ToList();


    /// <summary>(Median normalized price) x (target factor), rounded to nearest 10</summary>
    public static int? SuggestedPrice(IEnumerable<SoldSale> sales, Condition targetCondition)
    {
        var normalized = NormalizedPrices(sales);
        if (normalized.Count < MinSalesForSuggestion)
            return null;
        
        var raw = Statistics.Median(normalized) * ConditionFactors.For(targetCondition);
        return (int)(Math.Round(raw / 10m, MidpointRounding.AwayFromZero) * 10);
    }

    // Ai generated
    /// <summary>Low / Normal / High using the 1.5·IQR rule on normalized prices. Unknown if &lt; 5 sales.</summary>
    public static PriceAssessment Assess(int price, Condition condition, IEnumerable<SoldSale> sales)
    {
        var normalized = NormalizedPrices(sales);
        if (normalized.Count < MinSalesForAssessment)
            return PriceAssessment.Unknown;

        var q1 = Statistics.QuartileInc(normalized, 1);
        var q3 = Statistics.QuartileInc(normalized, 3);
        var iqr = q3 - q1;
        var normalizedPrice = ConditionFactors.Normalize(price, condition);

        if (normalizedPrice < q1 - 1.5m * iqr) return PriceAssessment.Low;
        if (normalizedPrice > q3 + 1.5m * iqr) return PriceAssessment.High;
        return PriceAssessment.Normal;
    }
    
    /// <summary>Get the old price, or the current price if it's never changed.</summary>
    public static int OriginalPrice(int currentPrice, IEnumerable<PriceChange> changes) =>
        changes.OrderBy(c => c.ChangedDate).ThenBy(c=> c.Id)
            .Select(c => (int?)c.OldPrice)
            .FirstOrDefault() ?? currentPrice;

    /// <summary>Get the average of original + sold price / original x 100.</summary>
    public static decimal? AverageDiscountPercent(IEnumerable<(int OriginalPrice, int SoldPrice)> sales)
    {
        var list = sales.ToList();
        if (list.Count == 0) return null;

        return list.Average(s => s.OriginalPrice - s.SoldPrice * 100m / s.OriginalPrice);
    }
}