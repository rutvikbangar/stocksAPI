using CarMarketplace.Api.Model;
using Grpc.Core;
using StocksGrpc;

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
        var request = MapToRequest(filters);

        try
        {
            var response = await _grpcClient.GetFilteredStocksAsync(request);
            return response.Stocks.Select(MapToStock);
        }
        catch (RpcException ex)
        {
            throw new StockRepositoryException("Failed to fetch stocks from Service.", ex);
        }
    }

    private static StockFilterRequest MapToRequest(Filters filters)
    {
        var request = new StockFilterRequest
        {
            Page = filters.Page,
            PageSize = filters.PageSize,
        };

        request.FuelTypes.AddRange(filters.FuelTypes.Select(f => (FuelTypeProto)(int)f));

        if (filters.MinBudget.HasValue)
        {
            request.MinBudget = (double)filters.MinBudget.Value;
        }

        if (filters.MaxBudget.HasValue)
        {
            request.MaxBudget = (double)filters.MaxBudget.Value;
        }

        if (filters.City is not null)
        {
            request.City = filters.City;
        }

        if (filters.MakeName is not null)
        {
            request.MakeName = filters.MakeName;
        }

        if (filters.SortBy.HasValue)
        {
            request.SortBy = (SortTypeProto)(int)filters.SortBy.Value;
        }

        return request;
    }

    private static Stock MapToStock(StockProto proto)
    {
        return new Stock
        {
            Id = proto.Id,
            MakeName = proto.MakeName,
            ModelName = proto.ModelName,
            MakeYear = proto.MakeYear,
            ImageUrl = proto.HasImageUrl ? proto.ImageUrl : null,
            Price = (decimal)proto.Price,
            FuelType = (FuelType)(int)proto.FuelType,
            KmsDriven = proto.KmsDriven,
            City = proto.City,
        };
    }
}