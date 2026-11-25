using ExchangeMarket.Api.DTOs.Listings;
using ExchangeMarket.Api.DTOs.Offers;
using ExchangeMarket.Api.Exceptions;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Models.Enums;
using ExchangeMarket.Api.Repositories;
using ExchangeMarket.Api.Service;
using Microsoft.EntityFrameworkCore;

namespace ExchangeMarket.Tests;

public class BusinessRuleTests : IDisposable
{
    private readonly TestDb _db = new();
    private readonly OfferService _offers;
    private readonly ListingService _listings;
    private readonly Listing _listing; 

    public BusinessRuleTests()
    {
        var ctx = _db.Context;
        var listingRepo = new ListingRepository(ctx);
        var analytics = new AnalyticsService(new AnalyticsRepository(ctx));

        _offers = new OfferService(new OfferRepository(ctx), listingRepo);
        _listings = new ListingService(listingRepo, analytics);

        ctx.Users.AddRange(User("seller"), User("buyer1"), User("buyer2"));
        _listing = new Listing
        {
            Title = "Test bike",
            Price = 1000,
            Condition = Condition.Good,
            Status = ListingStatus.Active,
            Category = new Category { Name = "Test" },
            SellerId = "seller"
        };
        ctx.Listings.Add(_listing);
        ctx.SaveChanges();
    }

    public void Dispose() => _db.Dispose();

    // utils
    private static ApplicationUser User(string id) =>
        new() { Id = id, UserName = $"{id}@test.no", Email = $"{id}@test.no", FullName = id };

    private static CreateOfferDto Bid(int amount) => new() { Amount = amount };

    private ListingRequestDto Edit(int price) => new()
    {
        Title = _listing.Title, Price = price, Condition = Condition.Good, CategoryId = _listing.CategoryId
    };

    private async Task MarkListingSold()
    {
        _listing.Status = ListingStatus.Sold;
        await _db.Context.SaveChangesAsync();
    }

    // offers
    [Fact]
    public async Task Offer_on_own_listing_is_rejected() =>
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _offers.PlaceAsync(_listing.Id, Bid(800), "seller"));

    [Fact]
    public async Task Offer_on_sold_listing_is_rejected()
    {
        await MarkListingSold();

        await Assert.ThrowsAsync<ConflictException>(() =>
            _offers.PlaceAsync(_listing.Id, Bid(800), "buyer1"));
    }

    [Fact]
    public async Task Second_pending_offer_from_same_buyer_is_rejected()
    {
        await _offers.PlaceAsync(_listing.Id, Bid(600), "buyer1");

        await Assert.ThrowsAsync<ConflictException>(() =>
            _offers.PlaceAsync(_listing.Id, Bid(700), "buyer1"));
    }

    [Fact]
    public async Task Offer_below_50_percent_is_rejected() =>
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _offers.PlaceAsync(_listing.Id, Bid(499), "buyer1"));

    [Fact]
    public async Task Offer_at_exactly_50_percent_is_accepted()
    {
        var offer = await _offers.PlaceAsync(_listing.Id, Bid(500), "buyer1");

        Assert.Equal(OfferStatus.Pending, offer.Status);
    }

    [Fact]
    public async Task Accepting_offer_sells_listing_and_rejects_other_pending_offers()
    {
        var winner = await _offers.PlaceAsync(_listing.Id, Bid(900), "buyer1");
        var loser  = await _offers.PlaceAsync(_listing.Id, Bid(800), "buyer2");

        await _offers.AcceptAsync(winner.Id, "seller");

        _db.Context.ChangeTracker.Clear();
        var listing = await _db.Context.Listings.SingleAsync(l => l.Id == _listing.Id);

        Assert.Equal(ListingStatus.Sold, listing.Status);
        Assert.Equal(900, listing.SoldPrice);
        Assert.NotNull(listing.SoldDate);
        Assert.Equal(OfferStatus.Accepted, (await _db.Context.Offers.SingleAsync(o => o.Id == winner.Id)).Status);
        Assert.Equal(OfferStatus.Rejected, (await _db.Context.Offers.SingleAsync(o => o.Id == loser.Id)).Status);
    }

    // listing
    [Fact]
    public async Task Price_change_creates_PriceChange_with_old_and_new_price()
    {
        await _listings.UpdateAsync(_listing.Id, Edit(price: 800), "seller", isAdmin: false);

        _db.Context.ChangeTracker.Clear();
        var change = await _db.Context.PriceChanges.SingleAsync(p => p.ListingId == _listing.Id);

        Assert.Equal(1000, change.OldPrice);
        Assert.Equal(800, change.NewPrice);
    }

    [Fact]
    public async Task Modifying_sold_listing_is_rejected()
    {
        await MarkListingSold();

        await Assert.ThrowsAsync<ConflictException>(() =>
            _listings.UpdateAsync(_listing.Id, Edit(price: 800), "seller", isAdmin: false));
    }
}