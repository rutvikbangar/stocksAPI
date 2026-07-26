using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Model;

namespace CarMarketplace.Api.Helpers;

public static class StockSearchValidator
{
public static (bool IsValid, string? ErrorMessage, Filters? Filters) Validate(
        SearchRequestDto stockSearch)
    {
        if (stockSearch.Page < 1)
            return (false, "page must be 1 or greater.", null);

        if (stockSearch.PageSize < 1 || stockSearch.PageSize > 50)
            return (false, "pageSize must be between 1 and 50.", null);

        if (stockSearch.CityId.HasValue && stockSearch.CityId.Value <= 0)
            return (false, "cityId must be a positive integer.", null);

        if (stockSearch.MakeId.HasValue && stockSearch.MakeId.Value <= 0)
            return (false, "makeId must be a positive integer.", null);

        List<decimal?> budgetList;
        try
        {
            budgetList = QueryParamParser.ParseBudget(stockSearch.Budget);
        }
        catch (FormatException ex)
        {
            return (false, ex.Message, null);
        }

        var (minBudget, maxBudget) = (budgetList[0], budgetList[1]);
        if (minBudget.HasValue && maxBudget.HasValue && minBudget.Value > maxBudget.Value)
            return (false, "budget minimum cannot be greater than maximum.", null);

        var fuelTypesList = new List<FuelType>();
        if (!string.IsNullOrWhiteSpace(stockSearch.FuelTypes))
        {
            foreach (var val in stockSearch.FuelTypes.Split(','))
            {
                if (!int.TryParse(val.Trim(), out var fuelTypeValue) ||
                    !Enum.IsDefined(typeof(FuelType), fuelTypeValue))
                {
                    return (false, $"Invalid fuelTypes value: '{val.Trim()}'.", null);
                }

                fuelTypesList.Add((FuelType)fuelTypeValue);
            }
        }

        SortType? sortBy = null;
        if (stockSearch.SortBy.HasValue)
        {
            if (!Enum.IsDefined(typeof(SortType), stockSearch.SortBy.Value))
                return (false, $"Invalid sortBy value: '{stockSearch.SortBy.Value}'.", null);

            sortBy = (SortType)stockSearch.SortBy.Value;
        }

        var filters = new Filters
        {
            MinBudget = minBudget,
            MaxBudget = maxBudget,
            FuelTypes = fuelTypesList,
            CityId = stockSearch.CityId,
            MakeId = stockSearch.MakeId,
            SortBy = sortBy,
            Page = stockSearch.Page,
            PageSize = stockSearch.PageSize
        };

        return (true, null, filters);
    }
   
}