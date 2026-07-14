namespace CarMarketplace.Api.Mappers;

using CarMarketplace.Api.Model;
using Riok.Mapperly.Abstractions;
using StocksGrpc;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByValue)]
public static partial class StockGrpcClientMapper
{
    // ---- Filters -> StockFilterRequest ----

    [MapperIgnoreTarget(nameof(StockFilterRequest.MinBudget))]
    [MapperIgnoreTarget(nameof(StockFilterRequest.MaxBudget))]
    [MapperIgnoreTarget(nameof(StockFilterRequest.City))]
    [MapperIgnoreTarget(nameof(StockFilterRequest.MakeName))]
    [MapperIgnoreTarget(nameof(StockFilterRequest.SortBy))]
    private static partial StockFilterRequest ToRequestCore(Filters filters);

    public static StockFilterRequest ToRequest(Filters filters)
    {
        var request = ToRequestCore(filters);

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

    // ---- StockProto -> Stock ----

    [MapperIgnoreTarget(nameof(Stock.ImageUrl))]
    public static partial Stock ToStockCore(StockProto proto);

    public static Stock ToStock(StockProto proto)
    {
        var stock = ToStockCore(proto);
        stock.ImageUrl = proto.HasImageUrl ? proto.ImageUrl : null;
        return stock;
    }
}