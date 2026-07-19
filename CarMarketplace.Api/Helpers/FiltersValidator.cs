using CarMarketplace.Api.Model;

namespace CarMarketplace.Api.Helpers;

public static class FiltersValidator
{
    public static (bool IsValid, string? ErrorMessage) Validate(Filters filters)
    {
        if (filters.Page < 1)
        {
            return (false, "page must be 1 or greater.");
        }

        if (filters.PageSize < 1 || filters.PageSize > 50)
        {
            return (false, "pageSize must be between 1 and 50.");
        }

        if (filters.CityId.HasValue && filters.CityId.Value <= 0)
        {
            return (false, "cityId must be a positive integer.");
        }

        if (filters.MakeId.HasValue && filters.MakeId.Value <= 0)
        {
            return (false, "makeId must be a positive integer.");
        }

        if (filters.MinBudget is < 0 || filters.MaxBudget is < 0)
        {
            return (false, "Budget values cannot be negative.");
        }

        if (filters.MinBudget.HasValue && filters.MaxBudget.HasValue
            && filters.MinBudget.Value > filters.MaxBudget.Value)
        {
            return (false, "budget minimum cannot be greater than maximum.");
        }

        foreach (var fuelType in filters.FuelTypes)
        {
            if (!Enum.IsDefined(typeof(FuelType), fuelType))
            {
                return (false, $"Invalid fuelTypes value: '{fuelType}'.");
            }
        }

        if (filters.SortBy.HasValue && !Enum.IsDefined(typeof(SortType), filters.SortBy.Value))
        {
            return (false, $"Invalid sortBy value: '{filters.SortBy.Value}'.");
        }

        return (true, null);
    }
}