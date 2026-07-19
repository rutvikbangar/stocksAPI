using GrpcMicroservice.Model;

namespace GrpcMicroservice.Helpers;

public static class FiltersValidator
{
    public static (bool IsValid, string? ErrorMessage) Validate(Filters filters)
    {
        if (filters.Page < 1)
        {
            return (false, "Page must be greater than 0.");
        }

        if (filters.PageSize < 1 || filters.PageSize > 50)
        {
            return (false, "PageSize must be between 1 and 50.");
        }

        if (filters.CityId is <= 0)
        {
            return (false, "CityId must be a positive integer.");
        }

        if (filters.MakeId is <= 0)
        {
            return (false, "MakeId must be a positive integer.");
        }

        if (filters.MinBudget is < 0)
        {
            return (false, "MinBudget cannot be negative.");
        }

        if (filters.MaxBudget is < 0)
        {
            return (false, "MaxBudget cannot be negative.");
        }

        if (filters.MinBudget.HasValue && filters.MaxBudget.HasValue
            && filters.MinBudget.Value > filters.MaxBudget.Value)
        {
            return (false, "MinBudget cannot be greater than MaxBudget.");
        }

        foreach (var fuelType in filters.FuelTypes)
        {
            if (!Enum.IsDefined(typeof(FuelType), fuelType))
            {
                return (false, $"Invalid fuel type value: '{fuelType}'.");
            }
        }

        if (filters.SortBy.HasValue && !Enum.IsDefined(typeof(SortType), filters.SortBy.Value))
        {
            return (false, $"Invalid sortBy value: '{filters.SortBy.Value}'.");
        }

        return (true, null);
    }
}