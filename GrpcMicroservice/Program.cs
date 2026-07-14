using GrpcMicroservice.Model;
using GrpcMicroservice.Repository;
using GrpcMicroservice.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockService, StockService>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5051, o =>
    {
        o.Protocols = HttpProtocols.Http2;
        o.UseHttps(); 
    });
});

var app = builder.Build();

app.MapGrpcService<StocksGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();