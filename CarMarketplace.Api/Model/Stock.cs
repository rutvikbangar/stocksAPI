namespace CarMarketplace.Api.Model;

public class Stock
{
    public int Id { get; set; }

    public string MakeName { get; set; } = string.Empty;

    public string ModelName { get; set; } = string.Empty;

    public int MakeYear { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    public FuelType FuelType { get; set; }

    public int KmsDriven { get; set; }

    public string City { get; set; } = string.Empty;
}

