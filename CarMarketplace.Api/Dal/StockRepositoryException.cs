namespace CarMarketplace.Api.Dal;

public class StockRepositoryException : Exception
{
    public StockRepositoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}