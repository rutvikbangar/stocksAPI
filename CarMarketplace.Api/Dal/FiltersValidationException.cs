namespace CarMarketplace.Api.Dal;

public class FiltersValidationException : Exception
{
    public FiltersValidationException(string message) : base(message) { }
}