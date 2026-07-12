using CarMarketplace.Api.Bal;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Helpers;
using CarMarketplace.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace CarMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

/// GET /api/stocks/search?budget=10000-200000&city=Mumbai&makeName=Maruti&fuelTypes=0,1&sortBy=0&page=1&pageSize=8
/// 
/// VALIDATION SPEARATE 
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
        [FromQuery] string? city,
        [FromQuery] string? makeName,
        [FromQuery] string? fuelTypes,
        [FromQuery] int? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 8)
    {
        if (page < 1)
        {
            return BadRequest("page must be 1 or greater.");
        }

        if (pageSize < 1 || pageSize > 50)
        {
            return BadRequest("pageSize must be between 1 and 50.");
        }

        List<decimal?> budgetList;
        try
        {
            budgetList = QueryParamParser.ParseBudget(budget);
        }
        catch (FormatException ex)
        {
            return BadRequest(ex.Message);
        }

        if (budgetList.Count == 2 && budgetList[0].HasValue && budgetList[1].HasValue
            && budgetList[0]!.Value > budgetList[1]!.Value)
        {
            return BadRequest("budget minimum cannot be greater than maximum.");
        }

        var fuelTypesList = new List<int>();
        if (!string.IsNullOrWhiteSpace(fuelTypes))
        {
            foreach (var val in fuelTypes.Split(','))
            {
                if (!int.TryParse(val.Trim(), out var fuelTypeValue) ||
                    !Enum.IsDefined(typeof(FuelType), fuelTypeValue))
                {
                    return BadRequest($"Invalid fuelTypes value: '{val.Trim()}'.");
                }

                fuelTypesList.Add(fuelTypeValue);
            }
        }

        if (sortBy.HasValue && !Enum.IsDefined(typeof(SortType), sortBy.Value))
        {
            return BadRequest($"Invalid sortBy value: '{sortBy.Value}'.");
        }

        var filtersDto = new FiltersDto
        {
            Budget = budgetList,
            City = city,
            MakeName = makeName,
            FuelTypes = fuelTypesList,
            SortBy = sortBy,
            Page = page,
            PageSize = pageSize
        };

        var result = await _stockService.GetFilteredStocksAsync(filtersDto);


        return Ok(result);
    }
}