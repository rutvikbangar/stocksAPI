extern alias GrpcMs;
using GrpcMs::GrpcMicroservice.Model;
using GrpcMs::GrpcMicroservice.Services;
using GrpcMs::StocksGrpc;
using Grpc.Core;
using Grpc.Core.Testing;
using Moq;
using Xunit;

namespace CarMarketplace.Tests;

public class StocksGrpcServiceTests
{
    private readonly Mock<IStockService> _mockStockService;
    private readonly StocksGrpcService _service;
    private readonly ServerCallContext _context;

    public StocksGrpcServiceTests()
    {
        _mockStockService = new Mock<IStockService>();
        _service = new StocksGrpcService(_mockStockService.Object);
        _context = TestServerCallContext.Create(
            method: "GetFilteredStocks", host: "localhost", deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: new Metadata(), cancellationToken: CancellationToken.None,
            peer: "localhost", authContext: null, contextPropagationToken: null,
            writeHeadersFunc: _ => Task.CompletedTask, writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: _ => { });
    }

    [Fact]
    public async Task GetFilteredStocks_ValidRequest_ReturnsMappedStockList()
    {
        var request = new StockFilterRequest { Page = 1, PageSize = 8 };

        _mockStockService
            .Setup(s => s.GetFilteredStocksAsync(It.IsAny<Filters>()))
            .ReturnsAsync(new List<Stock>
            {
                new Stock { Id = 1, ModelName = "Swift", ImageUrl = "http://img/1.jpg" },
                new Stock { Id = 2, ModelName = "City", ImageUrl = null }
            });

        var response = await _service.GetFilteredStocks(request, _context);

        Assert.Equal(2, response.Stocks.Count);
        Assert.Equal("Swift", response.Stocks[0].ModelName);
        Assert.Equal("http://img/1.jpg", response.Stocks[0].ImageUrl);
    }


    [Fact]
    public async Task GetFilteredStocks_MinBudgetGreaterThanMaxBudget_ThrowsInvalidArgument()
    {
        var request = new StockFilterRequest { Page = 1, PageSize = 8, MinBudget = 500000, MaxBudget = 100000 };

        var ex = await Assert.ThrowsAsync<RpcException>(
            () => _service.GetFilteredStocks(request, _context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("MinBudget cannot be greater than MaxBudget", ex.Status.Detail);
    }

    [Fact]
    public async Task GetFilteredStocks_NegativeCityId_ThrowsInvalidArgument()
    {
        var request = new StockFilterRequest { Page = 1, PageSize = 8, CityId = -3 };

        var ex = await Assert.ThrowsAsync<RpcException>(
            () => _service.GetFilteredStocks(request, _context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("CityId must be a positive integer", ex.Status.Detail);
    }


    [Fact]
    public async Task GetFilteredStocks_ServiceThrowsApplicationException_MapsToInternalStatus()
    {
        var request = new StockFilterRequest { Page = 1, PageSize = 8 };

        _mockStockService
            .Setup(s => s.GetFilteredStocksAsync(It.IsAny<Filters>()))
            .ThrowsAsync(new ApplicationException("DB connection failed"));

        var ex = await Assert.ThrowsAsync<RpcException>(
            () => _service.GetFilteredStocks(request, _context));

        Assert.Equal(StatusCode.Internal, ex.StatusCode);
        Assert.Equal("DB connection failed", ex.Status.Detail);
    }

}