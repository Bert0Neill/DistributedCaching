using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Caching.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caching.CommonCode.Classes
{
    public class MongoDBDistributedService : IDistributedService
    {
        private readonly IDistributedCache _agnosticDistributedCache;
        private readonly ILogger<MongoDBDistributedService> _logger;
        private readonly SqlServerEmployeeDatabaseContext _dbContext; // source of data (SQl Server)
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public MongoDBDistributedService(IDistributedCache agnosticDistributedCache, ILogger<MongoDBDistributedService> logger, SqlServerEmployeeDatabaseContext dbContext)
        {
            _logger = logger;
            _agnosticDistributedCache = agnosticDistributedCache;
            _dbContext = dbContext;

            // create a new DistributedCacheEntryOptions instance
            _cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(2), // Set absolute expiration to a specific date and time
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5), // Set relative expiration to 5 minutes from now
                SlidingExpiration = TimeSpan.FromMinutes(2) // Set sliding expiration to 2 minutes
            };
        }

        public async Task<Employee?> RetrieveEmployeeByNameAsync(string employeeName, CancellationToken cancellationToken = default)        
        {
            Employee? employee;
            string key = $"employee-{employeeName}";
            _logger.LogInformation(key);

            // check Redis cache first (cache hit)
            string? cachedJsonEmployee = await _agnosticDistributedCache.GetStringAsync(key, cancellationToken);

            // check if employee doesn't exist in cacahe - then go to DB (cache miss)
            if (string.IsNullOrEmpty(cachedJsonEmployee))
            {
                // query DB
                employee = await _dbContext.Employees
                                  .AsNoTracking()
                                  .Where(e => e.EmployeeName == employeeName)                                  
                                   .FirstOrDefaultAsync(cancellationToken);

                if (employee is null) return employee; // not found in DB either

                // found in DB and cache for futrure calls
                await _agnosticDistributedCache.SetStringAsync(
                    key,
                    JsonConvert.SerializeObject(employee),
                    _cacheOptions, // include eviction option
                    cancellationToken);

                return employee;
            }
            else // found in cache
            {
                employee = JsonConvert.DeserializeObject<Employee>(cachedJsonEmployee);
                return employee;
            }
        }
    }
}
