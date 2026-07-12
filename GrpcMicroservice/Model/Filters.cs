namespace GrpcMicroservice.Model;

public class Filters
{
    public decimal? MinBudget { get; set; }

    public decimal? MaxBudget { get; set; }

    public List<FuelType> FuelTypes { get; set; } = new List<FuelType>();

    public string? City { get; set; }

    public string? MakeName { get; set; }

    public SortType? SortBy { get; set; }

    public int Page {get;set;} = 1;

    public int PageSize {get;set;} = 8;
}


