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

// default health check - ping
app.MapGet("/", () => "Healthy - Caching.Redis_API_Server_1");

// inject EmployeeDatabaseContext & ILogger
app.MapGet("/Top10Employee", async (ILogger<Program> logger, EmployeeDatabaseContext dbContext) =>
{
    logger.LogInformation("Top10Employee called");

    var entities = await dbContext.Employees.Take(10).AsNoTracking().ToListAsync();
    return Results.Ok(entities);
});

app.MapGet("/GetEmployeeByName/{employeeName}", async (ILogger<Program> logger, EmployeeDatabaseContext dbContext, string employeeName) =>
{
    var message = $"Retrieving data for employee {employeeName}";
    logger.LogInformation(message);

    // query DB
    var employee = await dbContext.Employees
        .AsNoTracking()
        .Where(e => e.EmployeeName == employeeName)
        .FirstOrDefaultAsync();

    if (employee == null)
    {
        message = $"Employee with name {employeeName} not found.";
        logger.LogInformation(message);
        return Results.NotFound(message);
    }

    return Results.Ok(employee);
})
.WithName("GetEmployeeByName")
.WithOpenApi();

/************************************************************
 *              End Minimal API's                           *
 ***********************************************************/

app.Logger.LogInformation("Starting Caching.Redis_API_Server_1 app ...");
app.Run();