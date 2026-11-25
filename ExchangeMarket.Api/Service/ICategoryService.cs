using ExchangeMarket.Api.DTOs.Categories;

namespace ExchangeMarket.Api.Service;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CategoryRequestDto dto);
    Task<CategoryDto> UpdateAsync(int id, CategoryRequestDto dto);
    Task DeleteAsync(int id);
    
}