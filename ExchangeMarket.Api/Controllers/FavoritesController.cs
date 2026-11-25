using System.Security.Claims;
using ExchangeMarket.Api.DTOs.Listings;
using ExchangeMarket.Api.Models.Entities;
using ExchangeMarket.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeMarket.Api.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController(IFavoriteService service) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<List<ListingSummaryDto>>> GetMy() =>
        Ok(await service.GetMyAsync(UserId));

    [HttpPost]
    public async Task<IActionResult> Add(int listingId)
    {
        await service.AddAsync(UserId, listingId);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(int listingId)
    {
        await service.RemoveAsync(UserId, listingId);
        return NoContent();
    }
}