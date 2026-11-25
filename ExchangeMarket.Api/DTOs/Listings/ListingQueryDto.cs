using System.ComponentModel.DataAnnotations;
using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.DTOs.Listings;

public class ListingQueryDto
{
    [MaxLength(100)] public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public Condition? Condition { get; set; }

    [Range(0, int.MaxValue)] public int? MinPrice { get; set; }
    [Range(0, int.MaxValue)] public int? MaxPrice { get; set; }

    [RegularExpression("^(newest|price_asc|price_desc)$",
        ErrorMessage = "Sort must be 'newest' or 'price_asc' or 'price_desc'.")]
    public string? SortBy { get; set; }

}