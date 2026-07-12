namespace CarMarketplace.Api.Bal;

public class StockServiceException : Exception
{
    public StockServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}