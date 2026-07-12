using CarMarketplace.Api.Bal;
using CarMarketplace.Api.Dal;
using CarMarketplace.Api.Mappers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpcClient<StocksGrpc.StocksService.StocksServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["StocksService:Address"]!);
});

builder.Services.AddScoped<IStockRepository, StockGrpcRepository>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddSingleton<FiltersMapper>();
builder.Services.AddSingleton<StockMapper>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();