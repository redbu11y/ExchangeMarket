using System.Security.Claims;
using ExchangeMarket.Api.DTOs.Offers;
using ExchangeMarket.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeMarket.Api.Controllers;

[ApiController]
[Authorize]
public class OffersController(IOfferService service) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    
    [HttpGet("api/listings/{listingId:int}/offers")]
    public async Task<ActionResult<List<OfferDto>>> GetForListing(int listingId) =>
        Ok(await service.GetForListingAsync(listingId, UserId));

    [HttpPost("api/listings/{listingId:int}/offers")]
    public async Task<ActionResult<OfferDto>> Place(int listingId, CreateOfferDto dto)
    {
        var offer = await service.PlaceAsync(listingId, dto, UserId);
        return StatusCode(StatusCodes.Status201Created, offer);
    }
    
    [HttpGet("api/offers/my")]
    public async Task<ActionResult<List<OfferDto>>> GetMy() =>
        Ok(await service.GetMyAsync(UserId));
    
    [HttpPost("api/offers/{id:int}/accept")]
    public async Task<ActionResult<OfferDto>> Accept(int id) =>
        Ok(await service.AcceptAsync(id, UserId));
    
    [HttpPost("api/offers/{id:int}/reject")]
    public async Task<ActionResult<OfferDto>> Reject(int id) =>
        Ok(await service.RejectAsync(id, UserId));
    
    [HttpPost("api/offers/{id:int}/withdraw")]
    public async Task<ActionResult<OfferDto>> Withdraw(int id) =>
        Ok(await service.WithdrawAsync(id, UserId));
}