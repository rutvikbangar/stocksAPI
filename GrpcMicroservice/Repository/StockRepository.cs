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
        var sql = "SELECT * FROM Stocks";
        return await connection.QueryAsync<Stock>(sql);
    }

    public async Task<IEnumerable<Stock>> GetFilteredStocksAsync(Filters filters)
    {
        var connectionString = _configuration.GetConnectionString("StocksDb");
        using var connection = new MySqlConnection(connectionString);

        var (whereClause, parameters) = BuildWhereClause(filters);

        var sql = new StringBuilder("SELECT * FROM Stocks");
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

  
    private static (string Clause, DynamicParameters Parameters) BuildWhereClause(Filters filters)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (filters.MinBudget.HasValue)
        {
            conditions.Add("Price >= @MinBudget");
            parameters.Add("MinBudget", filters.MinBudget.Value);
        }

        if (filters.MaxBudget.HasValue)
        {
            conditions.Add("Price <= @MaxBudget");
            parameters.Add("MaxBudget", filters.MaxBudget.Value);
        }

        if (filters.FuelTypes is { Count: > 0 })
        {
            conditions.Add("FuelType IN @FuelTypes");
            parameters.Add("FuelTypes", filters.FuelTypes.Select(f => (int)f));
        }

        if (!string.IsNullOrWhiteSpace(filters.City))
        {
            conditions.Add("City = @City");
            parameters.Add("City", filters.City);
        }

        if (!string.IsNullOrWhiteSpace(filters.MakeName))
        {
            conditions.Add("MakeName = @MakeName");
            parameters.Add("MakeName", filters.MakeName);
        }

        var clause = conditions.Count > 0
            ? " WHERE " + string.Join(" AND ", conditions)
            : string.Empty;

        return (clause, parameters);
    }

    private static string BuildOrderByClause(SortType? sortBy)
    {
        return sortBy == SortType.Desc
            ? " ORDER BY Price DESC"
            : " ORDER BY Price ASC";
    }
}