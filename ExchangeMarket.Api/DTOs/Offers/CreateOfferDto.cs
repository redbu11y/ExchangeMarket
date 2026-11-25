using System.ComponentModel.DataAnnotations;

namespace ExchangeMarket.Api.DTOs.Offers;

public class CreateOfferDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be a positive number.")]
    public int Amount { get; set; }
}