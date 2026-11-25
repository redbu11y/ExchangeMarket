using ExchangeMarket.Api.DTOs.Categories;
using ExchangeMarket.Api.Exceptions;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Repositories;

namespace ExchangeMarket.Api.Service;

public class CategoryService(ICategoryRepository repo) : ICategoryService
{
    public async Task<List<CategoryDto>> GetAllAsync() =>
        (await repo.GetAllAsync()).Select(ToDto).ToList();

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await repo.GetByIdAsync(id)
                       ?? throw new NotFoundException($"Category with id {id} not found");
        return ToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CategoryRequestDto dto)
    {
        var name = dto.Name.Trim();
        if (await repo.NameExistsAsync(name))
            throw new ConflictException($"A category named '{name}' already exists'");
        
        var category = new Category { Name = name };
        repo.Add(category);
        await repo.SaveChangesAsync();
        return ToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, CategoryRequestDto dto)
    {
        var category = await repo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category with id {id} not found");
        
        var name = dto.Name.Trim();
        if (await repo.NameExistsAsync(name, excludeId: id))
            throw new ConflictException($"A category named '{name}' already exists'");
        
        category.Name = name.Trim();
        await repo.SaveChangesAsync();
        return ToDto(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await repo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category {id} was not found.");

        if (await repo.HasListingAsync(id))
            throw new ConflictException("Cannot delete a category that contains listings.");

        repo.Remove(category);
        await repo.SaveChangesAsync();
    }
    
    private static CategoryDto ToDto(Category c) => new(c.Id, c.Name);
}