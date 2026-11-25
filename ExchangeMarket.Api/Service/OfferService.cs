using ExchangeMarket.Api.DTOs.Offers;
using ExchangeMarket.Api.Exceptions;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;
using ExchangeMarket.Api.Repositories;

namespace ExchangeMarket.Api.Service;

public class OfferService(IOfferRepository offers, IListingRepository listings) : IOfferService
{
    public async Task<List<OfferDto>> GetForListingAsync(int listingId, string userId)
    {
        var listing = await listings.GetByIdAsync(listingId)
            ?? throw new NotFoundException($"Listing with id {listingId} does not exist");
        
        if (listing.SellerId != userId)
            throw new ForbiddenException("Only the seller can view offers on this listing.");
        
        return (await offers.GetForListingAsync(listingId)).Select(ToDto).ToList();
    }
    
    public async Task<List<OfferDto>> GetMyAsync(string userId) =>
        (await offers.GetByBuyerAsync(userId)).Select(ToDto).ToList();

    public async Task<OfferDto> PlaceAsync(int listingId, CreateOfferDto dto, string buyerId)
    {
        var listing = await listings.GetByIdAsync(listingId)
            ?? throw new NotFoundException($"Listing with id {listingId} does not exist");
        
        //Man kan ikke gi bud på egen annonse (400).
        if (listing.SellerId == buyerId)
            throw new BadRequestException("You cannot place an offer on your own listing");
        
        //Bud kan bare gis på annonser med status Active (409).
        if(listing.Status != ListingStatus.Active)
            throw new ConflictException("You can only place an offer on an active listing");
        
        //En kjøper kan bare ha ett Pending-bud per annonse (409).
        if (await offers.HasPendingAsync(listingId, buyerId))
            throw new ConflictException("You already have an offer on this listing");

        //Et bud må være minst 50 % av annonsens pris (400).
        if ((long)dto.Amount * 2 < listing.Price)
            throw new BadRequestException("The offer must be at least 50% of the listed price");

        var offer = new Offer
        {
            ListingId = listingId,
            BuyerId = buyerId,
            Amount = dto.Amount,
            Status = OfferStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };
        
        offers.Add(offer);
        
        await offers.SaveChangesAsync();
        return await GetDtoAsync(offer.Id);
    }

    public async Task<OfferDto> AcceptAsync(int offerId, string userId)
    {
        var offer = await GetOfferForSellerAsync(offerId, userId);
        var listing = offer.Listing;
        
        if (listing.Status != ListingStatus.Active)
            throw new ConflictException("The listing is no longer active.");
        
        await using var transaction = await offers.BeginTransactionAsync();

        offer.Status = OfferStatus.Accepted;

        listing.Status = ListingStatus.Sold;
        listing.SoldPrice = offer.Amount;
        listing.SoldDate = DateTime.UtcNow;

        foreach (var other in await offers.GetOtherPendingAsync(listing.Id, offer.Id))
            other.Status = OfferStatus.Rejected;
        
        await offers.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return await GetDtoAsync(offer.Id);
    }

    public async Task<OfferDto> RejectAsync(int offerId, string userId)
    {
        var offer = await GetOfferForSellerAsync(offerId, userId);

        offer.Status = OfferStatus.Rejected;
        await offers.SaveChangesAsync();
        return await GetDtoAsync(offer.Id);
    }

    public async Task<OfferDto> WithdrawAsync(int offerId, string userId)
    {
        var offer = await offers.GetWithListingAsync(offerId)
            ?? throw new NotFoundException($"Offer with id {offerId} does not exist");
        
        if (offer.BuyerId != userId)
            throw new BadRequestException("Only the buyer can withdraw this offer");
        
        if (offer.Status != OfferStatus.Pending)
            throw new ConflictException("Only pending offer can be withdrawn.");

        offer.Status = OfferStatus.Withdrawn;
        await offers.SaveChangesAsync();
        return await GetDtoAsync(offer.Id);
    }

        
    private async Task<Offer> GetOfferForSellerAsync(int offerId, string userId)
    {
        var offer = await offers.GetWithListingAsync(offerId)
            ?? throw new NotFoundException($"Offer with id {offerId} does not exist.");
        
        if (offer.Listing.SellerId != userId)
            throw new ForbiddenException("Only the seller can accept/reject this offer.");
        
        if (offer.Status != OfferStatus.Pending)
            throw new ConflictException("Only pending offer can be accepted/rejected.");
      
        return offer;
    }
    
    private async Task<OfferDto> GetDtoAsync(int offerId) =>
        ToDto((await offers.GetDetailsAsync(offerId))!);

    private static OfferDto ToDto(Offer o) => new(
        o.Id, o.ListingId, o.Listing.Title, o.BuyerId,
        string.IsNullOrWhiteSpace(o.Buyer.FullName) ? o.Buyer.Email! : o.Buyer.FullName, o.Amount, o.Status,
        o.CreatedDate);

}