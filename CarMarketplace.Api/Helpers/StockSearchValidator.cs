using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Model;

namespace CarMarketplace.Api.Helpers;

public static class StockSearchValidator
{
    public static (bool IsValid, string? ErrorMessage, FiltersDto? Filters) Validate(
        string? budget,
        int? cityId,
        int? makeId,
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

        if (cityId.HasValue && cityId.Value <= 0)
        {
            return (false, "cityId must be a positive integer.", null);
        }

        if (makeId.HasValue && makeId.Value <= 0)
        {
            return (false, "makeId must be a positive integer.", null);
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
            CityId = cityId,
            MakeId = makeId,
            FuelTypes = fuelTypesList,
            SortBy = sortBy,
            Page = page,
            PageSize = pageSize
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