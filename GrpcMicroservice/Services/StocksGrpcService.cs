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
    {
        if (request is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Request cannot be null."));
        }

        ValidateRequest(request);

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

    private static void ValidateRequest(StockFilterRequest request)
    {
        var errors = new List<string>();

        if (request.Page <= 0)
        {
            errors.Add("Page must be greater than 0.");
        }

        if (request.PageSize <= 0)
        {
            errors.Add("PageSize must be greater than 0.");
        }
        else if (request.PageSize > 50)
        {
            errors.Add("PageSize cannot exceed 50.");
        }

        if (request.HasMinBudget && request.MinBudget < 0)
        {
            errors.Add("MinBudget cannot be negative.");
        }

        if (request.HasMaxBudget && request.MaxBudget < 0)
        {
            errors.Add("MaxBudget cannot be negative.");
        }

        if (request.HasMinBudget && request.HasMaxBudget && request.MinBudget > request.MaxBudget)
        {
            errors.Add("MinBudget cannot be greater than MaxBudget.");
        }

        if (request.HasCityId && request.CityId <= 0)
        {
            errors.Add("CityId must be a positive integer.");
        }

        if (request.HasMakeId && request.MakeId <= 0)
        {
            errors.Add("MakeId must be a positive integer.");
        }

        if (errors.Count > 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join(" ", errors)));
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