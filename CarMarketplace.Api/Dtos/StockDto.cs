namespace CarMarketplace.Api.Dtos;

public class StockDto
{
    public int Id { get; set; }
    public string MakeName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int MakeYear { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public int KmsDriven { get; set; }
    public string City { get; set; } = string.Empty;

    // Computed fields
    public string FormattedPrice { get; set; } = string.Empty;
    public string CarName { get; set; } = string.Empty;
    public bool IsValueForMoney { get; set; }
}