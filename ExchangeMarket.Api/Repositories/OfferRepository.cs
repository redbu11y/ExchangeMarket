using ExchangeMarket.Api.Data;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExchangeMarket.Api.Repositories;

public class OfferRepository(AppDbContext db) : IOfferRepository
{
    public Task<Offer?> GetWithListingAsync(int offerId) =>
        db.Offers
            .Include(o => o.Listing)
            .FirstOrDefaultAsync(o => o.Id == offerId);

    public Task<Offer?> GetDetailsAsync(int offerId) =>
        db.Offers
            .AsNoTracking()
            .Include(o => o.Listing)
            .Include(o => o.Buyer)
            .FirstOrDefaultAsync(o => o.Id == offerId);
    
    public Task<List<Offer>> GetForListingAsync(int listingId) =>
        db.Offers
            .AsNoTracking()
            .Include(o => o.Listing)
            .Include(o => o.Buyer)
            .Where(o => o.ListingId == listingId)
            .OrderByDescending(o => o.Amount)
            .ToListAsync();

    public Task<List<Offer>> GetByBuyerAsync(string buyerId) =>
        db.Offers
            .AsNoTracking()
            .Include(o => o.Listing)
            .Include(o => o.Buyer)
            .Where(o => o.BuyerId == buyerId)
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();

    public Task<bool> HasPendingAsync(int listingId, string buyerId) =>
        db.Offers.AnyAsync(o => 
            o.ListingId == listingId &&
            o.BuyerId == buyerId &&
            o.Status == OfferStatus.Pending);

    public Task<List<Offer>> GetOtherPendingAsync(int listingId, int exceptOfferId) =>
        db.Offers
            .Where(o => o.ListingId == listingId &&
                        o.Id != exceptOfferId &&
                        o.Status == OfferStatus.Pending)
            .ToListAsync();
    
    public void Add(Offer offer) =>
        db.Offers.Add(offer);
    
    public Task SaveChangesAsync() =>
        db.SaveChangesAsync();
    
    public Task<IDbContextTransaction> BeginTransactionAsync() =>
        db.Database.BeginTransactionAsync();
}