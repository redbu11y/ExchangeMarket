namespace ExchangeMarket.Api.Models.Entities;

public class Favorite
{   
    public int Id { get; set; }
    
    public int ListingId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public Listing Listing { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    
}