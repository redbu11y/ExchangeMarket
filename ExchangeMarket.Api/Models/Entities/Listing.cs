using System.ComponentModel.DataAnnotations;
using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.Models.Entities;

public class Listing
{
    public int Id { get; set; }
    [MaxLength(100)] public required string Title { get; set; }
    [MaxLength(2000)] public required string Description { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Must be a whole number greater than 0.")]
    public required int Price { get; set; }
    
    public Condition Condition  { get; set; }
    public OfferStatus Status { get; set; } // Not sure about this one

    public string ImageUrl { get; set; } = "";
    
    public DateTime CreatedDate { get; set; }
    public DateTime SoldDate { get; set; }
    
    public int SoldPrice { get; set; }
    
    public int CategoryId { get; set; } // string?
    public int SellerId { get; set; }

}