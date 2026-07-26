using System.Reflection;
using CarMarketplace.Api.Bal;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
    public async Task<IActionResult> GetFiltered()
    {

        /*
        [FromQuery] string? budget,
        [FromQuery] int? cityId,
        [FromQuery] int? makeId,
        [FromQuery] string? fuelTypes,
        [FromQuery] int? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 8
        */

        var stockSearchRequestDto = MapSearchRequestDto(Request.Query, out var mappingErrors);

        if (mappingErrors.Any())
        {
            return BadRequest(string.Join(" ", mappingErrors));
        }

        var (isValid, errorMessage, filters) = StockSearchValidator.Validate(
            stockSearchRequestDto);

        if (!isValid)
        {
            return BadRequest(errorMessage);
        }

        try
        {
            var result = await _stockService.GetFilteredStocksAsync(filters!);
            return Ok(result);
        }
        catch (StockValidationException ex)
        {
            return BadRequest(ex.Message);
        }

    }

    private static SearchRequestDto MapSearchRequestDto(IQueryCollection query, out List<string> mappingErrors)
    {
        mappingErrors = new List<string>();
        var stockSearchRequestDto = new SearchRequestDto();
        PropertyInfo[] properties = typeof(SearchRequestDto).GetProperties();

        foreach (PropertyInfo prop in properties)
        {
            string name = prop.Name;
            if (!query.TryGetValue(name, out StringValues rawValues)) continue;

            string rawValue = rawValues.ToString();
            if (string.IsNullOrWhiteSpace(rawValue)) continue;

            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            try
            {
                object converted = targetType == typeof(string) ? rawValue : Convert.ChangeType(rawValue, targetType);
                prop.SetValue(stockSearchRequestDto, converted);
            }
            catch (Exception ex) when (ex is FormatException or OverflowException or InvalidCastException)
            {
                mappingErrors.Add($"Invalid value for '{prop.Name}': '{rawValue}'.");
            }
        }

        return stockSearchRequestDto;
    }
}