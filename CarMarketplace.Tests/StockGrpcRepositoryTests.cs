using CarMarketplace.Api.Dal;
using CarMarketplace.Api.Model;
using Grpc.Core;
using Grpc.Core.Testing;
using Moq;
using StocksGrpc;
using Xunit;

namespace CarMarketplace.Tests;

public class StockGrpcRepositoryTests
{
    private readonly Mock<StocksService.StocksServiceClient> _mockGrpcClient;
    private readonly StockGrpcRepository _repository;

    public StockGrpcRepositoryTests()
    {
        _mockGrpcClient = new Mock<StocksService.StocksServiceClient>();
        _repository = new StockGrpcRepository(_mockGrpcClient.Object);
    }

    private static AsyncUnaryCall<T> BuildAsyncUnaryCall<T>(T response) =>
        TestCalls.AsyncUnaryCall(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

    [Fact]
    public async Task GetFilteredStocksAsync_ValidResponse_ReturnsMappedStocks()
    {
        var filters = new Filters { CityId = 3, MakeId = 7, Page = 1, PageSize = 8 };

        var grpcResponse = new StockListResponse();
        grpcResponse.Stocks.Add(new StockProto { Id = 1, ModelName = "Swift", MakeName = "Maruti", City = "Mumbai" });
        grpcResponse.Stocks.Add(new StockProto { Id = 2, ModelName = "City", MakeName = "Honda", City = "Pune" });

        _mockGrpcClient
            .Setup(c => c.GetFilteredStocksAsync(
                It.IsAny<StockFilterRequest>(), null, null, default))
            .Returns(BuildAsyncUnaryCall(grpcResponse));

        var result = (await _repository.GetFilteredStocksAsync(filters)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.ModelName == "Swift");
    }


    [Fact]
    public async Task GetFilteredStocksAsync_ImageUrlSet_MapsValue()
    {
        var filters = new Filters();
        var grpcResponse = new StockListResponse();
        grpcResponse.Stocks.Add(new StockProto { Id = 1, ModelName = "Alto", ImageUrl = "http://img/1.jpg" });

        _mockGrpcClient
            .Setup(c => c.GetFilteredStocksAsync(It.IsAny<StockFilterRequest>(), null, null, default))
            .Returns(BuildAsyncUnaryCall(grpcResponse));

        var result = (await _repository.GetFilteredStocksAsync(filters)).ToList();

        Assert.Equal("http://img/1.jpg", result[0].ImageUrl);
    }

    [Fact]
    public async Task GetFilteredStocksAsync_EmptyResponse_ReturnsEmptyCollection()
    {
        var filters = new Filters();
        _mockGrpcClient
            .Setup(c => c.GetFilteredStocksAsync(It.IsAny<StockFilterRequest>(), null, null, default))
            .Returns(BuildAsyncUnaryCall(new StockListResponse()));

        var result = await _repository.GetFilteredStocksAsync(filters);

        Assert.Empty(result);
    }

}