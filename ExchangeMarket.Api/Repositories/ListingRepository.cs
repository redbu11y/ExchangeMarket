using ExchangeMarket.Api.Data;
using ExchangeMarket.Api.DTOs.Listings;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExchangeMarket.Api.Repositories;

public class ListingRepository(AppDbContext db) : IListingRepository
{
    public async Task<List<Listing>> SearchActiveAsync(ListingQueryDto query)
    {
        IQueryable<Listing> q = db.Listings
            .AsNoTracking()
            .Include(l => l.Category)
            .Where(l => l.Status == ListingStatus.Active);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(l => l.Title.ToLower().Contains(term) ||
                (l.Description != null && l.Description.ToLower().Contains(term)));
        }
        
        if (query.CategoryId is not null)
            q = q.Where(l => l.CategoryId == query.CategoryId);
        
        if (query.Condition is not null)
            q = q.Where(l => l.Condition == query.Condition);
        
        if (query.MinPrice is not null)
            q = q.Where(l => l.Price >= query.MinPrice);
        
        if (query.MaxPrice is not null)
            q = q.Where(l => l.Price <= query.MaxPrice);

        q = query.Sort switch
        {
            "price_asc" => q.OrderBy(l => l.Price).ThenByDescending(l => l.CreatedDate),
            "price_desc" => q.OrderByDescending(l => l.Price).ThenByDescending(l => l.CreatedDate), 
            _ => q.OrderByDescending(l => l.CreatedDate) //default
        };
        
        return await q.ToListAsync();
    }
    
}