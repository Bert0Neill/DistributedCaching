using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Caching.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Caching.CommonCode.Classes
{
    public class MongoDBDistributedService : DistributedServiceBase<SqlServerEmployeeDatabaseContext>
    {
        private readonly ILogger<MongoDBDistributedService> _logger;
        private readonly SqlServerEmployeeDatabaseContext _dbContext;

        public MongoDBDistributedService(IDistributedCache agnosticDistributedCache,
                                           ILogger<MongoDBDistributedService> logger,
                                           SqlServerEmployeeDatabaseContext dbContext)
            : base(agnosticDistributedCache)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override ILogger BaseLogger => _logger;
        protected override SqlServerEmployeeDatabaseContext BaseDbContext => _dbContext;
    }
}
