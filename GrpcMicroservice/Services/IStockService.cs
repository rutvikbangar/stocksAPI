namespace GrpcMicroservice.Services;
using GrpcMicroservice.Model;

public interface IStockService
{
    Task<IEnumerable<Stock>> GetFilteredStocksAsync(Filters filters);
}