using ExchangeMarket.Api.Models;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;
using ExchangeMarket.Api.Service.Analytics;

namespace ExchangeMarket.Tests;

public class AnalyticsTests
{
    // Electronics sales (2000,3000,3000,4000,4000,4000,5000,6000)
    private static readonly List<SoldSale> Electronics =
    [
        new(4000, Condition.New),     // 4000 / 1.00
        new(3400, Condition.LikeNew), // 3400 / 0.85
        new(2100, Condition.Good),    // 2100 / 0.70
        new(2200, Condition.Used),    // 2200 / 0.55
        new(3500, Condition.Good),    // 3500 / 0.70
        new(1650, Condition.Used),    // 1650 / 0.55
        new(5100, Condition.LikeNew), // 5100 / 0.85
        new(600,  Condition.Defect),  //  600 / 0.30
    ];

    private static readonly List<decimal> OneToTen =
        Enumerable.Range(1, 10).Select(i => (decimal)i).ToList();

    // median
    [Fact]
    public void Median_odd_count_is_middle_value() =>
        Assert.Equal(5m, Statistics.Median([9m, 1m, 7m, 3m, 5m]));   // unsorted on purpose

    [Fact]
    public void Median_even_count_is_average_of_two_middle_values() =>
        Assert.Equal(5.5m, Statistics.Median(OneToTen));              // (5 + 6) / 2

    // price suggesting
    [Fact]
    public void Suggestion_normalizes_and_rounds_to_nearest_10()
    {
        List<SoldSale> sales =
        [
            new(850,  Condition.LikeNew), //  850 / 0.85 
            new(1100, Condition.Used),    // 1100 / 0.55 
            new(1050, Condition.Good),    // 1050 / 0.70
        ];

        // Median 1500 × 0.55 (Used) = 825 → 830 (midpoint rounds away from zero, not to 820)
        Assert.Equal(830, PriceAnalysis.SuggestedPrice(sales, Condition.Used));
    }

    [Fact]
    public void Suggestion_for_electronics_in_good_condition_is_2800() =>
        Assert.Equal(2800, PriceAnalysis.SuggestedPrice(Electronics, Condition.Good));   // 4000 × 0.70

    [Fact]
    public void Suggestion_with_fewer_than_3_sales_returns_null() =>
        Assert.Null(PriceAnalysis.SuggestedPrice(Electronics.Take(2), Condition.Good));

    // price assessment 
    [Theory]
    [InlineData(500,  Condition.Used, PriceAssessment.Low)]     //  909.09 < 1125
    [InlineData(2800, Condition.Good, PriceAssessment.Normal)]  // 4000
    [InlineData(4900, Condition.Good, PriceAssessment.High)]    // 7000 > 6125
    public void Assessment_classifies_price(int price, Condition condition, PriceAssessment expected) =>
        Assert.Equal(expected, PriceAnalysis.Assess(price, condition, Electronics));

    [Fact]
    public void Assessment_with_fewer_than_5_sales_is_unknown() =>
        Assert.Equal(PriceAssessment.Unknown, PriceAnalysis.Assess(2800, Condition.Good, Electronics.Take(4)));

    // average discount
    [Fact]
    public void Discount_uses_first_old_price_when_price_was_changed()
    {
        // 5000->4500->4200, sold for 4000.
        List<PriceChange> changes =
        [
            new() { Id = 2, OldPrice = 4500, NewPrice = 4200, ChangedDate = new DateTime(2026, 1, 18) },
            new() { Id = 1, OldPrice = 5000, NewPrice = 4500, ChangedDate = new DateTime(2026, 1, 15) },
        ];

        var original = PriceAnalysis.OriginalPrice(currentPrice: 4200, changes);
        var discount = PriceAnalysis.AverageDiscountPercent([(original, 4000)]);

        Assert.Equal(5000, original);
        Assert.Equal(20m, discount); 
    }
}