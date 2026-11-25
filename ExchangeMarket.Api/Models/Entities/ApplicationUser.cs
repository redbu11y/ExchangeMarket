using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ExchangeMarket.Api.Models.Entities;

public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string FullName { get; set; } = "";
    
    public ICollection<Listing> Listings { get; set; } = [];
    public ICollection<Offer> Offers { get; set; } = [];
    public ICollection<Favorite> Favorites { get; set; } = [];
}