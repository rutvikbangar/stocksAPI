using CarMarketplace.Api.Model;
using Grpc.Core;
using StocksGrpc;
using CarMarketplace.Api.Mappers;

namespace CarMarketplace.Api.Dal;

public class StockGrpcRepository : IStockRepository
{
    private readonly StocksGrpc.StocksService.StocksServiceClient _grpcClient;

    public StockGrpcRepository(StocksGrpc.StocksService.StocksServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<IEnumerable<Stock>> GetFilteredStocksAsync(Filters filters)
    {
        var request = StockGrpcClientMapper.ToRequest(filters);

        try
        {
            var response = await _grpcClient.GetFilteredStocksAsync(request);
            return response.Stocks.Select(StockGrpcClientMapper.ToStock);
        }
        catch (RpcException ex)
        {
            throw new StockRepositoryException("Failed to fetch stocks from Service.", ex);
        }
    }
}