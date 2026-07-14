namespace CarMarketplace.Api.Mappers;

using CarMarketplace.Api.Model;
using Riok.Mapperly.Abstractions;
using StocksGrpc;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByValue)]
public static partial class StockGrpcClientMapper
{
    // ---- Filters -> StockFilterRequest ----

    [MapperIgnoreSource(nameof(Filters.MinBudget))]
    [MapperIgnoreSource(nameof(Filters.MaxBudget))]
    [MapperIgnoreSource(nameof(Filters.CityId))]
    [MapperIgnoreSource(nameof(Filters.MakeId))]
    [MapperIgnoreSource(nameof(Filters.SortBy))]
    [MapperIgnoreTarget(nameof(StockFilterRequest.MinBudget))]
    [MapperIgnoreTarget(nameof(StockFilterRequest.MaxBudget))]
    [MapperIgnoreTarget(nameof(StockFilterRequest.CityId))]
    [MapperIgnoreTarget(nameof(StockFilterRequest.MakeId))]
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

        if (filters.CityId.HasValue)
        {
            request.CityId = filters.CityId.Value;
        }

        if (filters.MakeId.HasValue)
        {
            request.MakeId = filters.MakeId.Value;
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