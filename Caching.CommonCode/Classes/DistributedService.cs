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
    public class DistributedService : IDistributedService
    {
        private readonly IDistributedCache _agnosticDistributedCache;
        private readonly ILogger<DistributedService> _logger;
        private readonly EmployeeDatabaseContext _dbContext;

        public DistributedService(IDistributedCache agnosticDistributedCache, ILogger<DistributedService> logger, EmployeeDatabaseContext dbContext)
        {
            _logger = logger;
            _agnosticDistributedCache = agnosticDistributedCache;
            _dbContext = dbContext;
        }

        public async Task<Employee?> RetrieveEmployeeByNameAsync(string employeeName, CancellationToken cancellationToken = default)        
        {
            Employee? employee;
            string key = $"employee-{employeeName}";
            _logger.LogInformation(key);

            // create cache options
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3),// expire after a fixed time period (e.g., 3 minutes).
                SlidingExpiration = TimeSpan.FromMinutes(1) // expire if it hasn't been accessed for a specified time period (e.g., 1 minute).
            };

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
                    cacheOptions, // include eviction option
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
