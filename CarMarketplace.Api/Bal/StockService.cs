using CarMarketplace.Api.Dal;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Mappers;
using CarMarketplace.Api.Model;
using CarMarketplace.Api.Helpers;
namespace CarMarketplace.Api.Bal;

public class StockService : IStockService
{
    private const int MaxKmsNeeded = 10_000;
    private const decimal MaxPriceNeeded = 2_00_000m;

    private readonly IStockRepository _stockRepository;
    private readonly StockMapper _stockMapper;
    private readonly FiltersMapper _filtersMapper;

    public StockService(IStockRepository stockRepository, StockMapper stockMapper, FiltersMapper filtersMapper)
    {
        _stockRepository = stockRepository;
        _stockMapper = stockMapper;
        _filtersMapper = filtersMapper;
    }

    public async Task<IEnumerable<StockDto>> GetFilteredStocksAsync(FiltersDto dto)
    {
        var (isValid, errorMessage) = StockSearchValidator.ValidateFiltersDto(dto);
        if (!isValid)
        {
            throw new StockValidationException(errorMessage!);
        }
        
        Filters filters = _filtersMapper.ToFilters(dto);

        IEnumerable<Stock> stocks;
        try
        {
            stocks = await _stockRepository.GetFilteredStocksAsync(filters);
        }
        catch (StockRepositoryException ex)
        {
            throw new StockServiceException("Failed to retrieve filtered stocks.", ex);
        }

        var result = new List<StockDto>();

        foreach (var stock in stocks)
        {
            var stockDto = _stockMapper.ToDto(stock);
            stockDto.IsValueForMoney = stock.KmsDriven < MaxKmsNeeded && stock.Price < MaxPriceNeeded;
            result.Add(stockDto);
        }

        return result;
    }


}







