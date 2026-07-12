using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Model;
using Riok.Mapperly.Abstractions;

namespace CarMarketplace.Api.Mappers;

[Mapper]
public partial class StockMapper
{
    public StockDto ToDto(Stock stock)
    {
        var dto = MapCoreProps(stock);
        dto.FormattedPrice = FormatPrice(stock.Price);
        dto.CarName = $"{stock.MakeYear} {stock.MakeName} {stock.ModelName}";
        return dto;
    }

    // IsValueForMoney done is bal
    [MapperIgnoreTarget(nameof(StockDto.IsValueForMoney))]
    [MapperIgnoreTarget(nameof(StockDto.FormattedPrice))]
    [MapperIgnoreTarget(nameof(StockDto.CarName))]
    private partial StockDto MapCoreProps(Stock stock);



    private static string FormatPrice(decimal price)
    {
        return price switch
        {
            >= 1_00_00_000 => $"Rs. {price / 1_00_00_000:0.##} Cr",
            >= 1_00_000 => $"Rs. {price / 1_00_000:0.##} Lakh",
            _ => $"Rs. {price:N0}"
        };
    }
}