using System.Security.Claims;
using ExchangeMarket.Api.DTOs.Listings;
using ExchangeMarket.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeMarket.Api.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingsController(IListingService service) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.FindFirstValue(ClaimTypes.Role) == "Admin";
    
    [HttpGet]
    public async Task<ActionResult<List<ListingSummaryDto>>> Search([FromQuery] ListingQueryDto  query) =>
        Ok(await service.SearchAsync(query));
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ListingDetailDto>> GetById(int id) =>
        Ok(await service.GetByIdAsync(id));
    
    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<List<ListingSummaryDto>>> GetMy() =>
        Ok(await service.GetMyAsync(UserId));

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ListingSummaryDto>> Create(ListingRequestDto dto)
    {
        var created = await service.CreateAsync(dto, UserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ListingDetailDto>> Update(int id, ListingRequestDto dto) =>
        Ok(await service.UpdateAsync(id, dto, UserId, IsAdmin));

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Remove(int id)
    {
        await service.RemoveAsync(id, UserId, IsAdmin);
        return NoContent();
    }
}