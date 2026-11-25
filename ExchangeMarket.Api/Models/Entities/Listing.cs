using System.ComponentModel.DataAnnotations;
using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.Models.Entities;

public class Listing
{
    public int Id { get; set; }


    [Required, MaxLength(100)] 
    public required string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)] 
    public string? Description { get; set; }
    
    [Range(1, int.MaxValue)]
    public required int Price { get; set; }
    
    public Condition Condition  { get; set; }
    public ListingStatus Status { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime SoldDate { get; set; }
    
    public int? SoldPrice { get; set; }
    
    //F-Keys
    public int CategoryId { get; set; }
    public string SellerId { get; set; } = string.Empty;

    public Category Category { get; set; } = null!;
    public ApplicationUser Seller { get; set; } = null!;
    public ICollection<Offer> Offers { get; set; } = new List<Offer>(); 
    public ICollection<PriceChange> PriceChanges { get; set; } = new List<PriceChange>();
    public ICollection<Favorite> Favorites { get; set; } = [];

}