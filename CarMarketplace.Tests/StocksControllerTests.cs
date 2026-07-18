using CarMarketplace.Api.Bal;
using CarMarketplace.Api.Controllers;
using CarMarketplace.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
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

    [Fact]
    public async Task GetFiltered_ValidParameters_ReturnsOkWithServiceResult()
    {
        var expected = new List<StockDto>
        {
            new StockDto { Id = 1, MakeName = "Maruti", ModelName = "Swift", City = "Mumbai" }
        };

        _mockStockService
            .Setup(s => s.GetFilteredStocksAsync(It.IsAny<FiltersDto>()))
            .ReturnsAsync(expected);

        var result = await _controller.GetFiltered(
            budget: "100000-200000",
            cityId: 3,
            makeId: 7,
            fuelTypes: "0,1",
            sortBy: 0,
            page: 1,
            pageSize: 8);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, okResult.Value);
        _mockStockService.Verify(s => s.GetFilteredStocksAsync(It.IsAny<FiltersDto>()), Times.Once);
    }

    [Fact]
    public async Task GetFiltered_PageLessThanOne_ReturnsBadRequestAndSkipsService()
    {
        var result = await _controller.GetFiltered(
            budget: null, cityId: null, makeId: null, fuelTypes: null,
            sortBy: null, page: 0, pageSize: 8);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("page must be 1 or greater.", badRequest.Value);
        _mockStockService.Verify(s => s.GetFilteredStocksAsync(It.IsAny<FiltersDto>()), Times.Never);
    }


    [Fact]
    public async Task GetFiltered_NegativeCityId_ReturnsBadRequest()
    {
        var result = await _controller.GetFiltered(
            budget: null, cityId: -3, makeId: null, fuelTypes: null,
            sortBy: null, page: 1, pageSize: 8);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("cityId must be a positive integer.", badRequest.Value);
        _mockStockService.Verify(s => s.GetFilteredStocksAsync(It.IsAny<FiltersDto>()), Times.Never);
    }

    [Fact]
    public async Task GetFiltered_InvertedBudgetRange_ReturnsBadRequest()
    {
        var result = await _controller.GetFiltered(
            budget: "200000-100000", cityId: null, makeId: null, fuelTypes: null,
            sortBy: null, page: 1, pageSize: 8);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("budget minimum cannot be greater than maximum.", badRequest.Value);
        _mockStockService.Verify(s => s.GetFilteredStocksAsync(It.IsAny<FiltersDto>()), Times.Never);
    }
}