namespace GrpcMicroservice.Mapper;

using GrpcMicroservice.Model;
using Riok.Mapperly.Abstractions;
using StocksGrpc;


[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByValue)]
public static partial class StockGrpcMapper
{
    [MapperIgnoreSource(nameof(StockFilterRequest.MinBudget))]
    [MapperIgnoreSource(nameof(StockFilterRequest.HasMinBudget))]
    [MapperIgnoreSource(nameof(StockFilterRequest.MaxBudget))]
    [MapperIgnoreSource(nameof(StockFilterRequest.HasMaxBudget))]
    [MapperIgnoreSource(nameof(StockFilterRequest.CityId))]
    [MapperIgnoreSource(nameof(StockFilterRequest.HasCityId))]
    [MapperIgnoreSource(nameof(StockFilterRequest.MakeId))]
    [MapperIgnoreSource(nameof(StockFilterRequest.HasMakeId))]
    [MapperIgnoreSource(nameof(StockFilterRequest.SortBy))]
    [MapperIgnoreSource(nameof(StockFilterRequest.HasSortBy))]
    [MapperIgnoreTarget(nameof(Filters.MinBudget))]
    [MapperIgnoreTarget(nameof(Filters.MaxBudget))]
    [MapperIgnoreTarget(nameof(Filters.CityId))]
    [MapperIgnoreTarget(nameof(Filters.MakeId))]
    [MapperIgnoreTarget(nameof(Filters.SortBy))]
    [MapperIgnoreTarget(nameof(Filters.Page))]
    [MapperIgnoreTarget(nameof(Filters.PageSize))]
    private static partial Filters ToFiltersCore(StockFilterRequest request);

    public static Filters ToFilters(StockFilterRequest request)
    {
        var filters = ToFiltersCore(request);

        filters.MinBudget = request.HasMinBudget ? (decimal)request.MinBudget : null;
        filters.MaxBudget = request.HasMaxBudget ? (decimal)request.MaxBudget : null;
        filters.CityId = request.HasCityId ? request.CityId : null;
        filters.MakeId = request.HasMakeId ? request.MakeId : null;
        filters.SortBy = request.HasSortBy ? (SortType)(int)request.SortBy : null;
        filters.Page = request.Page > 0 ? request.Page : 1;
        filters.PageSize = request.PageSize > 0 ? request.PageSize : 8;

        return filters;
    }

    [MapperIgnoreTarget(nameof(StockProto.ImageUrl))]
    public static partial StockProto ToStockProto(Stock stock);
}