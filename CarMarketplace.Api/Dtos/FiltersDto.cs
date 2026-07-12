namespace CarMarketplace.Api.Dtos;

public class FiltersDto
{
    // [100,1000]
    public List<decimal?> Budget { get; set; } = new List<decimal?>();

    public List<int> FuelTypes { get; set; } = new List<int>();

    public string? City { get; set; }

    public string? MakeName { get; set; }

    public int? SortBy { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 8;
}