using CarMarketplace.Api.Model;

namespace CarMarketplace.Api.Dal;

public interface IStockRepository
{
    Task<IEnumerable<Stock>> GetFilteredStocksAsync(Filters filters);
}