using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Model;
using Riok.Mapperly.Abstractions;

namespace CarMarketplace.Api.Mappers;

[Mapper]
public partial class FiltersMapper
{
    public Filters ToFilters(FiltersDto dto)
    {
        var filters = MapCore(dto);

        filters.MinBudget = dto.Budget.Count > 0 ? dto.Budget[0] : null;
        filters.MaxBudget = dto.Budget.Count > 1 ? dto.Budget[1] : null;
        filters.FuelTypes = MapFuelTypes(dto.FuelTypes);
        filters.SortBy = dto.SortBy.HasValue ? (SortType)dto.SortBy.Value : null;

        return filters;
    }

    [MapperIgnoreSource(nameof(FiltersDto.Budget))]
    [MapperIgnoreSource(nameof(FiltersDto.FuelTypes))]
    [MapperIgnoreSource(nameof(FiltersDto.SortBy))]
    [MapperIgnoreTarget(nameof(Filters.MinBudget))]
    [MapperIgnoreTarget(nameof(Filters.MaxBudget))]
    [MapperIgnoreTarget(nameof(Filters.FuelTypes))]
    [MapperIgnoreTarget(nameof(Filters.SortBy))]
    private partial Filters MapCore(FiltersDto dto);

    private static List<FuelType> MapFuelTypes(List<int> fuelTypes)
    {
        return fuelTypes.Select(f => (FuelType)f).ToList();
    }
}