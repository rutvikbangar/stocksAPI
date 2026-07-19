namespace CarMarketplace.Api.Bal;

public class StockValidationException : Exception
{
    public StockValidationException(string message) : base(message) { }
}