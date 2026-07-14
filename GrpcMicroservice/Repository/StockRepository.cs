namespace GrpcMicroservice.Repository;

using System.Text;
using Dapper;
using GrpcMicroservice.Model;
using MySqlConnector;

public class StockRepository : IStockRepository
{
    private readonly IConfiguration _configuration;

    public StockRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<Stock>> GetAllStocksAsync()
    {
        var connectionString = _configuration.GetConnectionString("StocksDb");
        using var connection = new MySqlConnection(connectionString);
        return await connection.QueryAsync<Stock>(BaseSelectSql());
    }

    public async Task<IEnumerable<Stock>> GetFilteredStocksAsync(Filters filters)
    {
        /// DI implement
        var connectionString = _configuration.GetConnectionString("StocksDb");
        using var connection = new MySqlConnection(connectionString);

        var (whereClause, parameters) = BuildWhereClause(filters);

        var sql = new StringBuilder(BaseSelectSql());
        sql.Append(whereClause);
        sql.Append(BuildOrderByClause(filters.SortBy));
        sql.Append(" LIMIT @PageSize OFFSET @Offset");

        parameters.Add("PageSize", filters.PageSize);
        parameters.Add("Offset", (filters.Page - 1) * filters.PageSize);

        try
        {
            return await connection.QueryAsync<Stock>(sql.ToString(), parameters);
        }
        catch (MySqlException ex)
        {
            throw new ApplicationException("Failed to fetch stocks.", ex);
        }
    }

    private static string BaseSelectSql()
    {
        return @"
            SELECT
                s.Id,
                mk.Name AS MakeName,
                md.Name AS ModelName,
                s.MakeYear,
                s.ImageUrl,
                s.Price,
                s.FuelTypeId AS FuelType,
                s.KmsDriven,
                c.Name AS City
            FROM Stocks s
            JOIN Makes mk ON mk.Id = s.MakeId
            JOIN Models md ON md.Id = s.ModelId
            JOIN Cities c ON c.Id = s.CityId";
    }

    private static (string Clause, DynamicParameters Parameters) BuildWhereClause(Filters filters)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (filters.MinBudget.HasValue)
        {
            conditions.Add("s.Price >= @MinBudget");
            parameters.Add("MinBudget", filters.MinBudget.Value);
        }

        if (filters.MaxBudget.HasValue)
        {
            conditions.Add("s.Price <= @MaxBudget");
            parameters.Add("MaxBudget", filters.MaxBudget.Value);
        }

        if (filters.FuelTypes is { Count: > 0 })
        {
            conditions.Add("s.FuelTypeId IN @FuelTypes");
            parameters.Add("FuelTypes", filters.FuelTypes.Select(f => (int)f));
        }

        if (filters.CityId.HasValue)
        {
            conditions.Add("s.CityId = @CityId");
            parameters.Add("CityId", filters.CityId.Value);
        }

        if (filters.MakeId.HasValue)
        {
            conditions.Add("s.MakeId = @MakeId");
            parameters.Add("MakeId", filters.MakeId.Value);
        }

        var clause = conditions.Count > 0
            ? " WHERE " + string.Join(" AND ", conditions)
            : string.Empty;

        return (clause, parameters);
    }

    private static string BuildOrderByClause(SortType? sortBy)
    {
        return sortBy == SortType.Desc
            ? " ORDER BY s.Price DESC"
            : " ORDER BY s.Price ASC";
    }
}