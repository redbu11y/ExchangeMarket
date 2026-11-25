using System.ComponentModel.DataAnnotations;
using ExchangeMarket.Api.DTOs.Analytics;
using ExchangeMarket.Api.Models.Enums;
using ExchangeMarket.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeMarket.Api.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController(IAnalyticsService service) : ControllerBase
{
    [HttpGet("price-suggestion")]
    [Authorize]
    public async Task<ActionResult<PriceSuggestionDto>> PriceSuggestion(
        [FromQuery, Required] int? categoryId,
        [FromQuery, Required] Condition? condition) =>
        Ok(await service.GetPriceSuggestionAsync(categoryId!.Value, condition!.Value));

    [HttpGet("categories/{id:int}/stats")]
    public async Task<ActionResult<CategoryStatsDto>> CategoryStats(int id) =>
        Ok(await service.GetCategoryStatsAsync(id));

    [HttpGet("overview")]
    public async Task<ActionResult<List<CategoryStatsDto>>> Overview() =>
        Ok(await service.GetOverviewAsync());

    [HttpGet("listings/{id:int}/price-history")]
    public async Task<ActionResult<PriceHistoryDto>> PriceHistory(int id) =>
        Ok(await service.GetPriceHistoryAsync(id));
}