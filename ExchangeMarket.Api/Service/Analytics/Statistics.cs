namespace ExchangeMarket.Api.Service.Analytics;

public static class Statistics
{
    
    /// <summary>Middle Value: average of the two middle values when the count is even.</summary>
    public static decimal Median(IEnumerable<decimal> values)
    {
        var sorted = SortedOrThrow(values);
        var n = sorted.Count;
        var mid = n / 2;
        
        return n % 2 == 1
            ? sorted[mid]
            : (sorted[mid-1] + sorted[mid])/2m;
    }
    
    /// <summary>Average value</summary>
    public static decimal Mean(IEnumerable<decimal> values)
    {
        var list = values.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Need at least 1 value.", nameof(values));
        return list.Average();
    }

    private static List<decimal> SortedOrThrow(IEnumerable<decimal> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        if (sorted.Count == 0)
            throw new ArgumentException("Need at least 1 value.", nameof(values));
        return sorted;
    }

    // AI generated
    /*
     * index:   0     1     2     3     4     5     6     7
     * value: 2000  3000  3000  4000  4000  4000  5000  6000
     */
    /// <summary>Same as Excel QUARTILE.INC(values, quartile), quartile 0–4.</summary>
    public static decimal QuartileInc(IEnumerable<decimal> values, int quartile)
    {
        if (quartile is < 0 or > 4)
            throw new ArgumentOutOfRangeException(nameof(quartile), "Quartile must be 0–4.");

        var sorted = SortedOrThrow(values);

        // Position in the sorted list (0-based): (n − 1) · q/4
        var position = (sorted.Count - 1) * quartile / 4m;
        var lower = (int)Math.Floor(position);
        var fraction = position - lower;

        if (lower + 1 >= sorted.Count)
            return sorted[lower];

        // Linear interpolation between the two neighbours
        return sorted[lower] + fraction * (sorted[lower + 1] - sorted[lower]);
    }

    // AI generated
    /// <summary>Sample standard deviation (divides by n − 1).</summary>
    public static decimal SampleStandardDeviation(IEnumerable<decimal> values)
    {
        var list = values.ToList();
        if (list.Count < 2)
            throw new ArgumentException("At least two values are required.", nameof(values));

        var mean = list.Average();
        var sumOfSquares = list.Sum(v => (v - mean) * (v - mean));
        return (decimal)Math.Sqrt((double)(sumOfSquares / (list.Count - 1)));
    }
}