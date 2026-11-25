using ExchangeMarket.Api.Models.Entities;

namespace ExchangeMarket.Api.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category> GetByIdAsync(int id);
    
    Task<bool> NameExistsAsync(String name, int? excludeId = null);
    Task<bool> HasListingAsync(int id);
    
    void Add(Category category);
    void Remove(Category category);
    
    Task SaveChangesAsync();
    
}