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
    [FromQuery] string? cityId,
    [FromQuery] string? makeId,
    [FromQuery] string? fuelTypes,
    [FromQuery] string? sortBy,
    [FromQuery] string? page,
    [FromQuery] string? pageSize)
    {
        var (isValid, errorMessage, filtersDto) = StockSearchValidator.Validate(
            budget, cityId, makeId, fuelTypes, sortBy, page, pageSize);

        if (!isValid)
        {
            return BadRequest(errorMessage);
        }

        try
        {
            var result = await _stockService.GetFilteredStocksAsync(filtersDto!);
            return Ok(result);
        }
        catch (StockValidationException ex)
        {
            return BadRequest(ex.Message);
        }

    }
}