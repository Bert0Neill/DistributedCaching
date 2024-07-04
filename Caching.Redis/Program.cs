using Caching.EFCore.Database;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//// create references to builder sub-classes
//var config = builder.Configuration;
//var services = builder.Services;

// Dependency Injection
//builder.Services.AddTransient<IQueryHelper, QueryHelper>(); // DI helper class
builder.Services.AddDbContext<EmployeeDatabaseContext>();

// Configure Redis
builder.Services.AddStackExchangeRedisCache(options =>
{    
    options.Configuration = $"{builder.Configuration.GetValue<string>("RedisConnection:Host")}:{builder.Configuration.GetValue<string>("RedisConnection:Port")}";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/************************************************************
 *            Start Minimal API's                           *
 ***********************************************************/



//app.MapGet("/CacheServerHealth", async (IConnectionMultiplexer redis) =>
//{
//    var result = await redis.GetDatabase().PingAsync();
//    return result.CompareTo(TimeSpan.Zero) > 0 ? $"PONG: {result}" : null;
//})
//.WithName("CacheServerHealth")
//.WithOpenApi();

app.MapGet("/Top10Employee", async (EmployeeDatabaseContext dbContext) =>
{
    var entities = await dbContext.Employees.Take(10).AsNoTracking().ToListAsync();
    return Results.Ok(entities);
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

/************************************************************
 *              End Minimal API's                           *
 ***********************************************************/

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
