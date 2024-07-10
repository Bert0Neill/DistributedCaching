using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Caching.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Caching.CommonCode.Classes
{
    public class MySqlServerDistributedService : DistributedServiceBase<MySqlServerEmployeeDatabaseContext>
    {
        private readonly ILogger<MySqlServerDistributedService> _logger;
        private readonly MySqlServerEmployeeDatabaseContext _dbContext;

        public MySqlServerDistributedService(IDistributedCache agnosticDistributedCache,
                                           ILogger<MySqlServerDistributedService> logger,
                                           MySqlServerEmployeeDatabaseContext dbContext)
            : base(agnosticDistributedCache)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override ILogger Logger => _logger;
        protected override MySqlServerEmployeeDatabaseContext DbContext => _dbContext;
    }
}
