namespace CarMarketplace.Api.Helpers;

public static class QueryParamParser
{
    public static int? ParseNullableInt(string? raw, string paramName)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (!int.TryParse(raw.Trim(), out var value))
        {
            throw new FormatException($"{paramName} must be a valid integer: '{raw}'.");
        }

        return value;
    }

    public static int ParseIntWithDefault(string? raw, string paramName, int defaultValue)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return defaultValue;
        }

        if (!int.TryParse(raw.Trim(), out var value))
        {
            throw new FormatException($"{paramName} must be a valid integer: '{raw}'.");
        }

        return value;
    }

    public static List<decimal?> ParseBudget(string? budgetParam)
    {
        if (string.IsNullOrWhiteSpace(budgetParam))
        {
            return new List<decimal?> { null, null };
        }

        var hypIndx = budgetParam.IndexOf('-');
        if (hypIndx < 0)
        {
            throw new FormatException($"incorrect format :  '{budgetParam}'.");
        }

        var minSide = budgetParam[..hypIndx];
        var maxSide = budgetParam[(hypIndx + 1)..];

        if (string.IsNullOrWhiteSpace(minSide) && string.IsNullOrWhiteSpace(maxSide))
        {
            throw new FormatException($"incorrect format :  '{budgetParam}'.");
        }

        if (string.IsNullOrWhiteSpace(minSide))
        {
            throw new FormatException($"minimum budget must be specified: '{budgetParam}'.");
        }

        decimal? minBudget = null;
        decimal? maxBudget = null;

        if (!decimal.TryParse(minSide, out var min))
        {
            throw new FormatException($"Could not parse minimum budget '{minSide}'.");
        }

        minBudget = min;

        if (!string.IsNullOrWhiteSpace(maxSide))
        {
            if (!decimal.TryParse(maxSide, out var max))
            {
                throw new FormatException($"Could not parse maximum budget from '{maxSide}'.");
            }

            maxBudget = max;
        }

        if (minBudget is < 0 || maxBudget is < 0)
        {
            throw new FormatException("Budget values cannot be negative.");
        }

        return new List<decimal?> { minBudget, maxBudget };
    }
}