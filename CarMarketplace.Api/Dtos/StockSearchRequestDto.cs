namespace CarMarketplace.Api.Dtos;

public class SearchRequestDto
{
    public string? Budget { get; set; }
    public int? CityId { get; set; }
    public int? MakeId { get; set; }
    public string? FuelTypes { get; set; }
    public int? SortBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 8;
}