using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Model;

namespace CarMarketplace.Api.Helpers;

public static class StockSearchValidator
{
    public static (bool IsValid, string? ErrorMessage, FiltersDto? Filters) Validate(
        string? budget,
        string? cityId,
        string? makeId,
        string? fuelTypes,
        string? sortBy,
        string? page,
        string? pageSize)
    {
        int pageValue;
        int pageSizeValue;
        int? cityIdValue;
        int? makeIdValue;
        int? sortByValue;
        List<decimal?> budgetList;

        try
        {
            pageValue = QueryParamParser.ParseIntWithDefault(page, "page", 1);
            pageSizeValue = QueryParamParser.ParseIntWithDefault(pageSize, "pageSize", 8);
            cityIdValue = QueryParamParser.ParseNullableInt(cityId, "cityId");
            makeIdValue = QueryParamParser.ParseNullableInt(makeId, "makeId");
            sortByValue = QueryParamParser.ParseNullableInt(sortBy, "sortBy");
            budgetList = QueryParamParser.ParseBudget(budget);
        }
        catch (FormatException ex)
        {
            return (false, ex.Message, null);
        }

        if (pageValue < 1)
        {
            return (false, "page must be 1 or greater.", null);
        }

        if (pageSizeValue < 1 || pageSizeValue > 50)
        {
            return (false, "pageSize must be between 1 and 50.", null);
        }

        if (cityIdValue is <= 0)
        {
            return (false, "cityId must be a positive integer.", null);
        }

        if (makeIdValue is <= 0)
        {
            return (false, "makeId must be a positive integer.", null);
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

        if (sortByValue.HasValue && !Enum.IsDefined(typeof(SortType), sortByValue.Value))
        {
            return (false, $"Invalid sortBy value: '{sortByValue.Value}'.", null);
        }

        var filtersDto = new FiltersDto
        {
            Budget = budgetList,
            CityId = cityIdValue,
            MakeId = makeIdValue,
            FuelTypes = fuelTypesList,
            SortBy = sortByValue,
            Page = pageValue,
            PageSize = pageSizeValue
        };

        return (true, null, filtersDto);
    }
    public static (bool IsValid, string? ErrorMessage) ValidateFiltersDto(FiltersDto dto)
    {
        if (dto.Page < 1)
        {
            return (false, "page must be 1 or greater.");
        }

        if (dto.PageSize < 1 || dto.PageSize > 50)
        {
            return (false, "pageSize must be between 1 and 50.");
        }

        if (dto.CityId.HasValue && dto.CityId.Value <= 0)
        {
            return (false, "cityId must be a positive integer.");
        }

        if (dto.MakeId.HasValue && dto.MakeId.Value <= 0)
        {
            return (false, "makeId must be a positive integer.");
        }

        if (dto.Budget.Count != 0 && dto.Budget.Count != 2)
        {
            return (false, "budget must contain exactly two values (min and max).");
        }

        if (dto.Budget.Count == 2)
        {
            var (min, max) = (dto.Budget[0], dto.Budget[1]);

            if (min is < 0 || max is < 0)
            {
                return (false, "Budget values cannot be negative.");
            }

            if (min.HasValue && max.HasValue && min.Value > max.Value)
            {
                return (false, "budget minimum cannot be greater than maximum.");
            }
        }

        foreach (var fuelType in dto.FuelTypes)
        {
            if (!Enum.IsDefined(typeof(FuelType), fuelType))
            {
                return (false, $"Invalid fuelTypes value: '{fuelType}'.");
            }
        }

        if (dto.SortBy.HasValue && !Enum.IsDefined(typeof(SortType), dto.SortBy.Value))
        {
            return (false, $"Invalid sortBy value: '{dto.SortBy.Value}'.");
        }

        return (true, null);
    }



}