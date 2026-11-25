using System.ComponentModel.DataAnnotations;

namespace ExchangeMarket.Api.Models.Entities;

public class Category
{
    public int Id { get; set; }
    
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Listing>  Listings { get; set; } = [];
}