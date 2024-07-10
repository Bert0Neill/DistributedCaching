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
builder.Services.AddTransient<IDistributedService, SqlServerDistributedService>();
builder.Services.AddDbContext<SqlServerEmployeeDatabaseContext>();

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:Schema");
    options.TableName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:TableName");
});

// create response output caching (duration) policies - default is 20 deconds
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(10)));
    options.AddPolicy("Expire30", builder => builder.Expire(TimeSpan.FromSeconds(30)));
    options.AddPolicy("Expire60", builder => builder.Expire(TimeSpan.FromSeconds(60)));
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
app.MapGet("/", () => "Healthy - Caching.ResponseAPI_1");

// cache response for 30 seconds
app.MapGet("/GetEmployeeByName/{employeeName}", [OutputCache(PolicyName = "Expire30")] 
    async (IDistributedService agnosticDistributedCacheService, 
    ILogger<Program> logger, string employeeName) =>
{
    var message = $"Retrieving data for employee {employeeName}";
    logger.LogInformation(message);

    var employee = await agnosticDistributedCacheService.RetrieveEmployeeByNameAsync(employeeName);
    return Results.Ok(employee);
})
.WithName("GetEmployeeByName")
.WithOpenApi();

/************************************************************
 *              End Minimal API's                           *
 ***********************************************************/

app.Logger.LogInformation("Starting Caching.ResponseAPI_1 app ...");
app.Run();