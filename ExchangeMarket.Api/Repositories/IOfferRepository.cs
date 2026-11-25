using ExchangeMarket.Api.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExchangeMarket.Api.Repositories;

public interface IOfferRepository
{
    Task<Offer?> GetWithListingAsync(int offerId);
    Task<Offer?> GetDetailsAsync(int offerId);
    Task<List<Offer>> GetForListingAsync(int listingId);
    Task<List<Offer>> GetByBuyerAsync(string buyerId);
    Task<bool> HasPendingAsync (int listingId, string buyerId);
    Task<List<Offer>> GetOtherPendingAsync(int listingId, int exceptOfferId);
    void AddOffer(Offer offer);
    Task SaveChangesAsync();
    Task <IDbContextTransaction> BeginTransactionAsync();
    
}