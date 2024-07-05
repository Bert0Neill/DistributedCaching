using Microsoft.Extensions.Caching.Distributed;
using Alachisoft.NCache.Caching.Distributed;
using Alachisoft.NCache.Client;
using Caching.CommonCode.Classes;
using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddTransient<IDistributedService, DistributedService>();
builder.Services.AddDbContext<EmployeeDatabaseContext>();

// Configure NCache
builder.Services.AddNCacheDistributedCache(builder.Configuration.GetSection("NCacheSettings"));

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
app.MapGet("/", () => "Healthy - Caching.NCache_API_Server_1");

// inject EmployeeDatabaseContext & ILogger
app.MapGet("/Top10Employee", async (ILogger < Program> logger, EmployeeDatabaseContext dbContext) =>
{
    
    logger.LogInformation("Top10Employee called");

    var entities = await dbContext.Employees.Take(10).AsNoTracking().ToListAsync();
    return Results.Ok(entities);
});

app.MapGet("/GetEmployeeByName/{employeeName}", async (IDistributedService redisDistributedCache, ILogger<Program> logger, string employeeName) =>
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

app.Logger.LogInformation("Starting Caching.NCache_API_Server_1 app ...");
app.Run();
