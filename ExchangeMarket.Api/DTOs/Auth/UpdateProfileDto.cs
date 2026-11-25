using System.ComponentModel.DataAnnotations;

namespace ExchangeMarket.Api.DTOs.Auth;

public class UpdateProfileDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
}