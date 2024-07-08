using Caching.CommonCode.Classes;
using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddTransient<IDistributedService, MySqlServerDistributedService>(); // inject MySql service
builder.Services.AddDbContext<MySqlServerEmployeeDatabaseContext>(); // inject MySQL dbContext

builder.Services.AddDistributedMySqlCache(options =>
{    
    //options.ConnectionString = "server=127.0.0.1;uid=username;pwd=password;database=employeedatabase;port=3306;pooling=false;SslMode=None;old Guids=true;Charset=utf8;AllowUserVariables=true;";
    options.ConnectionString = "Server=127.0.0.1;Database=employeedatabase;User=bertoneill;Password=P@ssw0rd;port=3306;AllowUserVariables=true;";
    options.SchemaName = "employeedatabase";
    options.TableName = "mysqlcache"; // new Cache table

    //options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    //options.SchemaName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:Schema");
    //options.TableName = builder.Configuration.GetValue<string>("SqlDistributedCacheTable:TableName");
});

// create a new DistributedCacheEntryOptions instance
var cacheEntryOptions = new DistributedCacheEntryOptions
{
    AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10), // Set absolute expiration to a specific date and time
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5), // Set relative expiration to 5 minutes from now
    SlidingExpiration = TimeSpan.FromMinutes(2) // Set sliding expiration to 2 minutes
};

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

app.MapGet("/GetEmployeeByName/{employeeName}", async (MySqlServerEmployeeDatabaseContext _MySqlServerEmployeeDatabaseContext, IDistributedCache distributedCache, IDistributedService agnosticDistributedCacheService, ILogger<Program> logger, string employeeName) =>
{
    //// set
    //var key = "MyKey-1";
    //object value = new { id = 1, user = "zs" };
    //var stringObject = System.Text.Json.JsonSerializer.Serialize(value, new System.Text.Json.JsonSerializerOptions
    //{
    //    //.Net6+
    //    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    //});
    //await distributedCache.SetStringAsync(key, "123456", cacheEntryOptions);
    //distributedCache.Refresh(key);

    //// get
    //var getvlaue = await distributedCache.GetStringAsync(key);


    //var employees = await _MySqlServerEmployeeDatabaseContext.Employees
    //                           .AsNoTracking()
    //                           .Where(e => e.EmployeeName == employeeName)
    //                            .FirstOrDefaultAsync();


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
