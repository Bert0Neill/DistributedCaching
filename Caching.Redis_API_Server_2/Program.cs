using Caching.CommonCode.Classes;
using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddTransient<IDistributedService, DistributedService>(); // DI helper class
builder.Services.AddDbContext<EmployeeDatabaseContext>();

// Configure Redis Cache
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

// API health check - ping
app.MapGet("/", () => "Healthy - Caching.Redis_API_Server_2");

// inject EmployeeDatabaseContext & ILogger
app.MapGet("/Top10Employee", async (ILogger<Program> logger, EmployeeDatabaseContext dbContext) =>
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

app.Logger.LogInformation("Starting Caching.Redis_API_Server_2 app ...");
app.Run();
