using Caching.CommonCode.Interfaces;
using Caching.EFCore.Database;
using Caching.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Caching.CommonCode.Classes
{

    public class SqlServerDistributedService : DistributedServiceBase<SqlServerEmployeeDatabaseContext>
    {
        private readonly ILogger<SqlServerDistributedService> _logger;
        private readonly SqlServerEmployeeDatabaseContext _dbContext;

        public SqlServerDistributedService(IDistributedCache agnosticDistributedCache,
                                           ILogger<SqlServerDistributedService> logger,
                                           SqlServerEmployeeDatabaseContext dbContext)
            : base(agnosticDistributedCache)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override ILogger Logger => _logger;
        protected override SqlServerEmployeeDatabaseContext DbContext => _dbContext;
    }
}
