using ExchangeMarket.Api.Models.Entities;

namespace ExchangeMarket.Api.DTOs.Listings;

public static class ListingMappings
{
    public static ListingSummaryDto ToSummaryDto(this Listing l) => new(
        l.Id, l.Title, l.Price, l.Condition, l.Status, l.ImageUrl, l.CategoryId, l.Category.Name, l.CreatedDate);
}