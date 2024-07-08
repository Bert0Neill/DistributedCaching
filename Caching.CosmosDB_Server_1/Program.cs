using Caching.CommonCode.Classes;
using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Caching.EFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Response caching
builder.Services.AddOutputCache();

// Dependency Injection
builder.Services.AddTransient<IDistributedService, DistributedService>();
builder.Services.AddDbContext<EmployeeDatabaseContext>();

builder.Services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
{


    // PartionKey = /Demo
    cacheOptions.DatabaseName = "DemoDistributedCachingDB";
    cacheOptions.ContainerName = "Employee";    
    cacheOptions.ClientBuilder = new CosmosClientBuilder("AccountEndpoint=https://cosmosrgeastus6b99877b-fd91-4a41-a7eadb.documents.azure.com:443/;AccountKey=OqMddFAHhI8l5pAUFF3BPjqBgp03ilA63AvMFiJEB3IvVM9EdXIUzI1mr5toy75E2h3Pqhv7M28XACDb7pSW5A==;");
    cacheOptions.CreateIfNotExists = true;
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
app.MapGet("/", () => "Healthy - Caching.CosmosDB_Server_1");

// cache response for 60 seconds
app.MapGet("/GetEmployeeByName/{employeeName}", [OutputCache] async (IDistributedService agnosticDistributedCacheService, ILogger<Program> logger, string employeeName) =>
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

app.Logger.LogInformation("Starting Caching.CosmosDB_Server_1 app ...");
app.Run();