using Caching.CommonCode.Classes;
using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Response caching
builder.Services.AddOutputCache();

// Dependency Injection
builder.Services.AddTransient<IDistributedService, DistributedService>();
builder.Services.AddDbContext<EmployeeDatabaseContext>();

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:Schema");
    options.TableName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:TableName");
});

var app = builder.Build();

// Use aoutput caching
app.UseOutputCache();

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

// API health check - ping
app.MapGet("/", () => "Healthy - Caching.ResponseAPI_2");

// cache response for 60 seconds
app.MapGet("/Top10Employee", [OutputCache] async (ILogger<Program> logger, EmployeeDatabaseContext dbContext) =>
{
    logger.LogInformation("Top10Employee called");

    var entities = await dbContext.Employees.Take(10).AsNoTracking().ToListAsync();
    return Results.Ok(entities);
});

// cache response for 60 seconds
app.MapGet("/GetEmployeeByName/{employeeName}", [OutputCache] async (IDistributedService redisDistributedCache, ILogger<Program> logger, string employeeName) =>
{
    var message = $"Retrieving data for employee {employeeName}";
    logger.LogInformation(message);

    var employee = await redisDistributedCache.RetrieveEmployeeByNameAsync(employeeName);
    return Results.Ok(employee);
})
.WithName("GetEmployeeByName")
.WithOpenApi();

/************************************************************
 *              End Minimal API's                           *
 ***********************************************************/

app.Logger.LogInformation("Starting Caching.ResponseAPI_2 app ...");
app.Run();