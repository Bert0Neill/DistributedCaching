using Alachisoft.NCache.Caching.Distributed;
using Caching.CommonCode.Classes;
using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddTransient<IDistributedService, SqlServerDistributedService>(); // DI service class
builder.Services.AddDbContext<SqlServerEmployeeDatabaseContext>(); // DI database context

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

app.Logger.LogInformation("Starting Caching.NCache_API_Server_1 app ...");
app.Run();
