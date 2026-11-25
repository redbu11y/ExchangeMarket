using System.ComponentModel.DataAnnotations;
using ExchangeMarket.Api.Models.Enums;

namespace ExchangeMarket.Api.Models.Entities;

public class Offer
{
    public int Id { get; set; }
    
    public int ListingId { get; set; }
    
    [Required]
    public string BuyerId { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }
    
    public OfferStatus Status { get; set; } = OfferStatus.Pending;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public Listing Listing { get; set; } = null!;
    public ApplicationUser Buyer { get; set; } = null!;
}