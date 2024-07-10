using Caching.CommonCode.Interfaces;
using Caching.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Caching.CommonCode.Classes
{
    public abstract class DistributedServiceBase<TContext> : IDistributedService where TContext : DbContext
    {
        private readonly IDistributedCache _agnosticDistributedCache;

        protected DistributedServiceBase(IDistributedCache agnosticDistributedCache)
        {
            _agnosticDistributedCache = agnosticDistributedCache;
        }

        protected abstract ILogger Logger { get; }
        protected abstract TContext DbContext { get; }

        public async Task<Employee?> RetrieveEmployeeByNameAsync(string employeeName, CancellationToken cancellationToken = default)
        {
            Employee? employee;
            string key = $"employee-{employeeName}";
            Logger.LogInformation(key);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3),
                SlidingExpiration = TimeSpan.FromMinutes(1)
            };

            string? cachedJsonEmployee = await _agnosticDistributedCache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrEmpty(cachedJsonEmployee))
            {
                employee = await DbContext.Set<Employee>()
                                  .AsNoTracking()
                                  .Where(e => e.EmployeeName == employeeName)
                                  .FirstOrDefaultAsync(cancellationToken);

                if (employee is null) return employee;

                await _agnosticDistributedCache.SetStringAsync(
                    key,
                    JsonConvert.SerializeObject(employee),
                    cacheOptions,
                    cancellationToken);

                return employee;
            }
            else
            {
                employee = JsonConvert.DeserializeObject<Employee>(cachedJsonEmployee);
                return employee;
            }
        }
    }
}
