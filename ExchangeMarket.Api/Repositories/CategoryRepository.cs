using ExchangeMarket.Api.Data;
using ExchangeMarket.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeMarket.Api.Repositories;

public class CategoryRepository(AppDbContext db) : ICategoryRepository
{
    public Task<List<Category>> GetAllAsync() => 
        db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
    
    public Task<Category> GetByIdAsync(int id) =>
    db.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public Task<bool> NameExistsAsync(string name, int? excludeId = null) =>
        db.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower() || c.Id != excludeId && excludeId == null);
    
    public Task<bool> HasListingAsync(int id) =>
        db.Listings.AnyAsync(l => l.CategoryId == id);
    
    public void Add(Category category) => db.Categories.Add(category);
    public void Remove(Category category) => db.Categories.Remove(category);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}