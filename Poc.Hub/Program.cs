using Poc.Hub.Filters;
using Poc.Hub.Hub;
using Poc.Hub.Mapping;
using Poc.Hub.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var s = configuration.Sources;
var connectionMultiplexer = configuration.GetConnectionString("ConnectionMultiplexer"); 

builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMapping, RedisConnectionMapping>();
builder.Services.AddScoped<IClientRegistry, ClientRegistry>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionMultiplexer));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddControllers(options =>
// {
//     options.Filters.Add<HeaderAuthorization>();
// });
builder.Services.AddSignalR().AddStackExchangeRedis(connectionMultiplexer, options =>
{
    options.Configuration.ChannelPrefix = "ConsoleSignalRApp";
});

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<LogHub>("/Hub/LogHub");
app.MapControllers();

app.Run();