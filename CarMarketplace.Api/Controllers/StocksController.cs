using CarMarketplace.Api.Bal;
using CarMarketplace.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CarMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

/// GET /api/stocks/search?budget=10000-200000&cityId=3&makeId=7&fuelTypes=0,1&sortBy=0&page=1&pageSize=8
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;
    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetFiltered(
        [FromQuery] string? budget,
        [FromQuery] int? cityId,
        [FromQuery] int? makeId,
        [FromQuery] string? fuelTypes,
        [FromQuery] int? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 8)
    {
        var (isValid, errorMessage, filtersDto) = StockSearchValidator.Validate(
            budget, cityId, makeId, fuelTypes, sortBy, page, pageSize);

        if (!isValid)
        {
            return BadRequest(errorMessage);
        }

        var result = await _stockService.GetFilteredStocksAsync(filtersDto!);

        return Ok(result);
    }
}