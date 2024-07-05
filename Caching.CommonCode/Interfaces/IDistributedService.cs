using Caching.EFCore.Models;

namespace Caching.CommonCode.Interfaces
{
    public interface IDistributedService
    {
        Task<Employee?> RetrieveEmployeeByNameAsync(string employeeName, CancellationToken cancellationToken = default);
    }
}