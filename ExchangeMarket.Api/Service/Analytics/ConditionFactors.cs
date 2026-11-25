using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.Service.Analytics;

public static class ConditionFactors
{
    // Multiplier
    public static decimal For(Condition condition) => condition switch
    {
        Condition.New => 1.00m,
        Condition.LikeNew => 0.85m,
        Condition.Good => 0.70m,
        Condition.Used => 0.50m,
        Condition.Defect => 0.30m,
        _ => throw new ArgumentOutOfRangeException(nameof(condition))
    };
    
    // Original price
    public static decimal Normalize (int price, Condition condition) => price / For(condition);
}