using Caching.CommonCode.Classes;
using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDbCache;
using Newtonsoft.Json.Linq;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddTransient<IDistributedService, MongoDBDistributedService>(); // inject MongoDB service
builder.Services.AddDbContext<SqlServerEmployeeDatabaseContext>(); // inject SQL Server dbContext (data source)

// determine what MongoDB database & collection to use for caching (these will already be created in MongoDB)
builder.Services.AddMongoDbCache(options =>
{
    //options.ConnectionString = "mongodb://localhost:27017";
    //options.DatabaseName = "MongoCache";
    //options.CollectionName = "AppCache";

    options.ConnectionString = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:ConnectionString");
    options.DatabaseName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:Schema");
    options.CollectionName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:TableName");

    options.ExpiredScanInterval = TimeSpan.FromMinutes(10);
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

// API health check - ping
app.MapGet("/", () => "Healthy - Caching.MongoDB_API_Server_2");

app.MapGet("/GetEmployeeByName/{employeeName}", async (IDistributedService agnosticDistributedCacheService, ILogger<Program> logger, string employeeName) =>
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

app.Logger.LogInformation("Starting Caching.MongoDB_API_Server_2 app ...");
app.Run();
