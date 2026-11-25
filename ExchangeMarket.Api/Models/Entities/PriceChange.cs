
using System.ComponentModel.DataAnnotations;

namespace ExchangeMarket.Api.Models.Entities;

public class PriceChange
{
    public int Id { get; set; }
    
    public int  ListingId { get; set; }
    public int OldPrice { get; set; }
    public int NewPrice { get; set; }

    public DateTime ChangedDate { get; set; } = DateTime.UtcNow;

    public Listing Listing { get; set; } = null!;

}