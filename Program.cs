using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using background_jobs.Data;
using background_jobs.Services;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

var GetConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(GetConnectionString));


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "PharmacyAppCache:";

    // Add connection resilience
    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
    {
        EndPoints = { builder.Configuration.GetConnectionString("Redis")! },
        AbortOnConnectFail = false,
        ConnectRetry = 5,
        ConnectTimeout = 5000,
        SyncTimeout = 5000
    };
});

//add cors
builder.Services.AddCors(options =>
{
    // Define a specific policy
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") 
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });

    // Optionally, define a more permissive default policy (use with caution)
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddScoped<ICoinDataService, CoinDataService>();
builder.Services.AddHostedService<ExchangeRateUpdaterService>();
builder.Services.AddHttpClient("CoinGecko", client =>
{
    client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MyCryptoApp/1.0"); // Set the User-Agent
});


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();


app.Run();

