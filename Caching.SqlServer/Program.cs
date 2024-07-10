using Caching.CommonCode.Classes;
using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddTransient<IDistributedService, SqlServerDistributedService>(); // DI service class
builder.Services.AddDbContext<SqlServerEmployeeDatabaseContext>();  // DI database context

builder.Services.AddDistributedSqlServerCache(options =>
{    
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:Schema");
    options.TableName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:TableName"); 
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
app.MapGet("/", () => "Healthy - Caching.SqlServer_API_1");

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

app.Logger.LogInformation("Starting Caching.SqlServer_API_1 app ...");
app.Run();
