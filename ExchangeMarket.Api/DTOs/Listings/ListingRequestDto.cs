using System.ComponentModel.DataAnnotations;
using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.DTOs.Listings;

public class ListingRequestDto
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }
    
    [Required]
    public Condition? Condition { get; set; }
    
    [Url, MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId is required")]
    public int CategoryId { get; set; }
}