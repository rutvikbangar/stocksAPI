namespace GrpcMicroservice.Repository;

using GrpcMicroservice.Model;

public interface IStockRepository
{
    Task<IEnumerable<Stock>> GetAllStocksAsync();
    Task<IEnumerable<Stock>> GetFilteredStocksAsync(Filters filters);
}
