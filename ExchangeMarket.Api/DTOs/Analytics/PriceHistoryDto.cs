namespace ExchangeMarket.Api.DTOs.Analytics;

public record PriceChangeDto(int OldPrice, int NewPrice, DateTime ChangedDate);

public record PriceHistoryDto(
    int ListingId,
    string Title,
    int OriginalPrice,
    int CurrentPrice,
    List<PriceChangeDto> Changes
);