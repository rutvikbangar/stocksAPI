namespace GrpcMicroservice.Repository;

using System.Data;
using MySqlConnector;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("StocksDb")
            ?? throw new InvalidOperationException("Connection string 'StocksDb' is not configured.");
    }

    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}