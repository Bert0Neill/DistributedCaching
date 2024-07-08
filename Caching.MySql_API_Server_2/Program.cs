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
    ////options.ConnectionString = "server=127.0.0.1;uid=username;pwd=password;database=employeedatabase;port=3306;pooling=false;SslMode=None;old Guids=true;Charset=utf8;AllowUserVariables=true;";
    //options.ConnectionString = "Server=127.0.0.1;Database=employeedatabase;User=bertoneill;Password=P@ssw0rd;port=3306;AllowUserVariables=true;";
    //options.SchemaName = "employeedatabase";
    //options.TableName = "mysqlcache"; // new Cache table

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
app.MapGet("/", () => "Healthy - Caching.MySql_API_Server_2");

app.MapGet("/GetEmployeeByName/{employeeName}", async (MySqlServerEmployeeDatabaseContext _MySqlServerEmployeeDatabaseContext, IDistributedCache distributedCache, IDistributedService agnosticDistributedCacheService, ILogger<Program> logger, string employeeName) =>
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

app.Logger.LogInformation("Starting Caching.MySql_API_Server_2 app ...");
app.Run();
