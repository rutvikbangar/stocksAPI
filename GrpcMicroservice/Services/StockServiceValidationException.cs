namespace GrpcMicroservice.Services;

public class StockServiceValidationException : Exception
{
    public StockServiceValidationException(string message) : base(message) { }
}