using Ardalis.GuardClauses;
using Caching.CommonCode.Classes;
using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddTransient<IDistributedService, SqlServerDistributedService>(); // DI service class
builder.Services.AddDbContext<SqlServerEmployeeDatabaseContext>(); // DI database context

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

/*************************************************************************
* Associate a Global Error handler middleware for your end-points *
**************************************************************************/
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            // consistant output
            var problemDetails = new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Title = "An unexpected error occurred!",
                Detail = contextFeature.Error.Message
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    });
});

/************************************************************
 *            Start Minimal API's                           *
 ***********************************************************/

// API health check - ping
app.MapGet("/", () => "Healthy - Caching.Redis_API_Server_1");

app.MapGet("/GetEmployeeByName/{employeeName}", async (IDistributedService agnosticDistributedCacheService, ILogger < Program> logger, string employeeName) =>
{
    //guard clause
    employeeName = Guard.Against.NullOrEmpty(employeeName.Trim(), nameof(employeeName));

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

app.Logger.LogInformation("Starting Caching.Redis_API_Server_1 app ...");
app.Run();