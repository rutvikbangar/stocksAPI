namespace GrpcMicroservice.Services;
using GrpcMicroservice.Model;
using GrpcMicroservice.Repository;
using GrpcMicroservice.Helpers;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;

    public StockService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<IEnumerable<Stock>> GetFilteredStocksAsync(Filters filters)
    {
        var (isValid, errorMessage) = FiltersValidator.Validate(filters);
        if (!isValid)
        {
            throw new StockServiceValidationException(errorMessage!);
        }

        try
        {
            return await _stockRepository.GetFilteredStocksAsync(filters);
        }
        catch (ApplicationException ex)
        {
            throw new Exception("Failed to retrieve filtered stocks.", ex);
        }
    }
}