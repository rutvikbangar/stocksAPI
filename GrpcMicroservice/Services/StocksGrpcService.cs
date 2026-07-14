namespace GrpcMicroservice.Services;
using GrpcMicroservice.Model;
using Grpc.Core;
using StocksGrpc;
using GrpcMicroservice.Mapper;


public class StocksGrpcService : StocksGrpc.StocksService.StocksServiceBase
{
    private readonly IStockService _stockService;

    public StocksGrpcService(IStockService stockService)
    {
        _stockService = stockService;
    }

    public override async Task<StockListResponse> GetFilteredStocks(
        StockFilterRequest request,
        ServerCallContext context)
    { /// check
        var filters = StockGrpcMapper.ToFilters(request);

        try
        {
            var stocks = await _stockService.GetFilteredStocksAsync(filters);

            var response = new StockListResponse();
            response.Stocks.AddRange(stocks.Select(MapToStockProto));

            return response;
        }
        catch (ApplicationException ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    private static StockProto MapToStockProto(Stock stock)
    {
        var proto = StockGrpcMapper.ToStockProto(stock);

        if (stock.ImageUrl is not null)
        {
            proto.ImageUrl = stock.ImageUrl;
        }

        return proto;
    }
}