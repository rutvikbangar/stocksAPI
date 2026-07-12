namespace GrpcMicroservice.Services;
using GrpcMicroservice.Model;
using Grpc.Core;
using StocksGrpc;


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
        var filters = MapToFilters(request);

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

    private static Filters MapToFilters(StockFilterRequest request)
    {
        return new Filters
        {
            MinBudget = request.HasMinBudget ? (decimal)request.MinBudget : null,
            MaxBudget = request.HasMaxBudget ? (decimal)request.MaxBudget : null,
            FuelTypes = request.FuelTypes.Select(f => (FuelType)(int)f).ToList(),
            City = request.HasCity ? request.City : null,
            MakeName = request.HasMakeName ? request.MakeName : null,
            SortBy = request.HasSortBy ? (SortType)(int)request.SortBy : null,
            Page = request.Page > 0 ? request.Page : 1,
            PageSize = request.PageSize > 0 ? request.PageSize : 8,
        };
    }

    private static StockProto MapToStockProto(Stock stock)
    {
        var proto = new StockProto
        {
            Id = stock.Id,
            MakeName = stock.MakeName,
            ModelName = stock.ModelName,
            MakeYear = stock.MakeYear,
            Price = (double)stock.Price,
            FuelType = (FuelTypeProto)(int)stock.FuelType,
            KmsDriven = stock.KmsDriven,
            City = stock.City,
        };

        if (stock.ImageUrl is not null)
        {
            proto.ImageUrl = stock.ImageUrl;
        }

        return proto;
    }
}   