using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Model;

namespace CarMarketplace.Api.Helpers;

public static class StockSearchValidator
{
    public static (bool IsValid, string? ErrorMessage, FiltersDto? Filters) Validate(
        string? budget,
        string? city,
        string? makeName,
        string? fuelTypes,
        int? sortBy,
        int page,
        int pageSize)
    {
        if (page < 1)
        {
            return (false, "page must be 1 or greater.", null);
        }

        if (pageSize < 1 || pageSize > 50)
        {
            return (false, "pageSize must be between 1 and 50.", null);
        }

        List<decimal?> budgetList;
        try
        {
            budgetList = QueryParamParser.ParseBudget(budget);
        }
        catch (FormatException ex)
        {
            return (false, ex.Message, null);
        }

        if (budgetList.Count == 2 && budgetList[0].HasValue && budgetList[1].HasValue
            && budgetList[0]!.Value > budgetList[1]!.Value)
        {
            return (false, "budget minimum cannot be greater than maximum.", null);
        }

        var fuelTypesList = new List<int>();
        if (!string.IsNullOrWhiteSpace(fuelTypes))
        {
            foreach (var val in fuelTypes.Split(','))
            {
                if (!int.TryParse(val.Trim(), out var fuelTypeValue) ||
                    !Enum.IsDefined(typeof(FuelType), fuelTypeValue))
                {
                    return (false, $"Invalid fuelTypes value: '{val.Trim()}'.", null);
                }

                fuelTypesList.Add(fuelTypeValue);
            }
        }

        if (sortBy.HasValue && !Enum.IsDefined(typeof(SortType), sortBy.Value))
        {
            return (false, $"Invalid sortBy value: '{sortBy.Value}'.", null);
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

        return (true, null, filtersDto);
    }
}