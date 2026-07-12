namespace GrpcMicroservice.Services;
using GrpcMicroservice.Model;
using GrpcMicroservice.Repository;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;

    public StockService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<IEnumerable<Stock>> GetFilteredStocksAsync(Filters filters)
    {
        NormalizeFilters(filters);

        try
        {
            return await _stockRepository.GetFilteredStocksAsync(filters);
        }
        catch (ApplicationException ex)
        {
            throw new Exception("Failed to retrieve filtered stocks.", ex);
        }
    }

    private static void NormalizeFilters(Filters filters)
    {
        if (filters.Page < 1)
        {
            filters.Page = 1;
        }

        if (filters.PageSize is < 1 or > 50)
        {
            filters.PageSize = 8;
        }
    }
}