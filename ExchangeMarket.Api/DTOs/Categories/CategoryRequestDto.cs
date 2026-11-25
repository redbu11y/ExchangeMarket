using System.ComponentModel.DataAnnotations;

namespace ExchangeMarket.Api.DTOs.Categories;

public class CategoryRequestDto
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}