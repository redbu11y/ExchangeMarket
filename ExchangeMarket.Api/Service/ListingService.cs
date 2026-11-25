using ExchangeMarket.Api.DTOs.Listings;
using ExchangeMarket.Api.Exceptions;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;
using ExchangeMarket.Api.Repositories;

namespace ExchangeMarket.Api.Service;

public class ListingService(IListingRepository repo, IAnalyticsService analytics) : IListingService
{
    public async Task<List<ListingSummaryDto>> SearchAsync(ListingQueryDto query) =>
        (await repo.SearchActiveAsync(query)).Select(l => l.ToSummaryDto()).ToList();

public async Task<ListingDetailDto> GetByIdAsync(int id)
{
    var listing = await repo.GetDetailsAsync(id)
        ?? throw new NotFoundException($"Listing with id {id} not found.");

    var estimate = await analytics.GetListingEstimateAsync(listing.CategoryId, listing.Price, listing.Condition);
    return ToDetail(listing, estimate);
}

    public async Task<List<ListingSummaryDto>> GetMyAsync(string userId) =>
        (await repo.GetBySellerAsync(userId)).Select(l => l.ToSummaryDto()).ToList();
    
    public async Task<ListingDetailDto> CreateAsync(ListingRequestDto dto, string userId)
    {
        if (!await repo.CategoryExistsAsync(dto.CategoryId))
            throw new BadRequestException($"Category with id {dto.CategoryId} not found");

        var listing = new Listing
        {
            Title = dto.Title,
            Description = dto.Description?.Trim(),
            Price = dto.Price,
            Condition = dto.Condition!.Value,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            SellerId = userId,
            Status = ListingStatus.Active,
            CreatedDate = DateTime.UtcNow,
        };
        
        repo.Add(listing);
        await repo.SaveChangesAsync();
        return await GetByIdAsync(listing.Id);
    }

    // We save the old price before we overwrite it
    public async Task<ListingDetailDto> UpdateAsync(int id, ListingRequestDto dto, string userId, bool isAdmin)
    {
        var listing = await repo.GetByIdAsync(id)
            ?? throw new ArgumentException($"Listing with id {id} not found");
        
        EnsureCanModify(listing, userId, isAdmin);
        
        if (dto.CategoryId != listing.CategoryId && !await repo.CategoryExistsAsync(dto.CategoryId))
            throw new BadRequestException($"Category with id {dto.CategoryId} not found");

        if (dto.Price != listing.Price)
        {
            listing.PriceChanges.Add(new PriceChange
            {
                OldPrice = listing.Price,
                NewPrice = dto.Price,
                ChangedDate =  DateTime.UtcNow,
            });
            listing.Price = dto.Price;
        }
        
        listing.Title = dto.Title.Trim();
        listing.Description = dto.Description?.Trim();
        listing.Condition = dto.Condition!.Value;
        listing.ImageUrl = dto.ImageUrl;
        listing.CategoryId = dto.CategoryId;
        
        await repo.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task RemoveAsync(int id, string userId, bool isAdmin)
    {
        var listing = await repo.GetByIdAsync(id)
            ?? throw new ArgumentException($"Listing with id {id} not found");
        
        EnsureCanModify(listing, userId, isAdmin);
        
        listing.Status = ListingStatus.Removed;
        await repo.SaveChangesAsync();
    }

    private static void EnsureCanModify(Listing listing, string userId, bool isAdmin)
    {
        if (listing.SellerId != userId && !isAdmin)
            throw new ForbiddenException("Only the seller or admin can modify this listing");
        
        if (listing.Status == ListingStatus.Sold)
            throw new ConflictException("Sold listing cannot be modified");
        
        if (listing.Status == ListingStatus.Removed)
            throw new ConflictException("Removed listing cannot be modified");
    }
/*
    private static ListingSummaryDto ToSummary(Listing l) => new(
        l.Id, l.Title, l.Price, l.Condition, l.Status, l.ImageUrl, l.CategoryId, l.Category.Name, l.CreatedDate);
*/

private static ListingDetailDto ToDetail(Listing l, ListingEstimate estimate) => new(
    l.Id, l.Title, l.Description, l.Price, l.Condition, l.Status, l.ImageUrl,
    l.CreatedDate, l.SoldDate, l.SoldPrice,
    l.CategoryId, l.Category.Name,
    l.SellerId,
    string.IsNullOrWhiteSpace(l.Seller.FullName) ? l.Seller.Email! : l.Seller.FullName,
    l.Offers.Count,
    estimate.SuggestedPrice,
    estimate.Assessment.ToString());
}