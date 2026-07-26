using CarMarketplace.Api.Bal;
using CarMarketplace.Api.Controllers;
using CarMarketplace.Api.Model;
using CarMarketplace.Api.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace CarMarketplace.Tests;

public class StocksControllerTests
{
    private readonly Mock<IStockService> _mockStockService;
    private readonly StocksController _controller;

    public StocksControllerTests()
    {
        _mockStockService = new Mock<IStockService>();
        _controller = new StocksController(_mockStockService.Object);
    }

    private static void SetQuery(StocksController controller, Dictionary<string, string?> queryParams)
    {
        var dict = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in queryParams)
        {
            dict[kv.Key] = new StringValues(kv.Value);
        }

        var queryCollection = new QueryCollection(dict);

        var httpContext = new DefaultHttpContext
        {
            Request = { Query = queryCollection }
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }
    [Fact]
    public async Task GetFiltered_ValidParameters_ReturnsOkWithServiceResult()
    {
        SetQuery(_controller, new Dictionary<string, string?>
        {
            ["budget"] = "10000-200000",
            ["cityId"] = "3",
            ["makeId"] = "7",
            ["fuelTypes"] = "0,1",
            ["sortBy"] = "0",
            ["page"] = "1",
            ["pageSize"] = "8"
        });

        var expected = new List<StockDto>
        {
            new StockDto { Id = 1, MakeName = "Maruti", ModelName = "Swift", City = "Mumbai" }
        };

        _mockStockService
            .Setup(s => s.GetFilteredStocksAsync(It.IsAny<Filters>()))
            .ReturnsAsync(expected);

        var result = await _controller.GetFiltered();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, okResult.Value);
        _mockStockService.Verify(s => s.GetFilteredStocksAsync(It.IsAny<Filters>()), Times.Once);
    }

    [Fact]
    public async Task GetFiltered_PageLessThanOne_ReturnsBadRequestAndSkipsService()
    {
        SetQuery(_controller, new Dictionary<string, string?>
        {
            ["page"] = "0"
        });

        var result = await _controller.GetFiltered();

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("page must be 1 or greater.", badRequest.Value);
        _mockStockService.Verify(s => s.GetFilteredStocksAsync(It.IsAny<Filters>()), Times.Never);
    }

    [Fact]
    public async Task GetFiltered_NegativeCityId_ReturnsBadRequest()
    {
        SetQuery(_controller, new Dictionary<string, string?>
        {
            ["cityId"] = "-1"
        });

        var result = await _controller.GetFiltered();

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("cityId must be a positive integer.", badRequest.Value);
        _mockStockService.Verify(s => s.GetFilteredStocksAsync(It.IsAny<Filters>()), Times.Never);
    }

    [Fact]
    public async Task GetFiltered_InvertedBudgetRange_ReturnsBadRequest()
    {
        SetQuery(_controller, new Dictionary<string, string?>
        {
            ["budget"] = "200000-10000"
        });

        var result = await _controller.GetFiltered();

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("budget minimum cannot be greater than maximum.", badRequest.Value);
        _mockStockService.Verify(s => s.GetFilteredStocksAsync(It.IsAny<Filters>()), Times.Never);
    }
}