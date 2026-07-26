using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Model;

namespace CarMarketplace.Api.Bal;

public interface IStockService
{
    Task<IEnumerable<StockDto>> GetFilteredStocksAsync(Filters dto);
}