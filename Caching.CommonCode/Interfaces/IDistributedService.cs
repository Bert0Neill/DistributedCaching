using Caching.EFCore.Models;

namespace Caching.CommonCode.Interfaces
{
    public interface IDistributedService
    {
        // ToDo: Make fnx <T> for any object - expand out Open\Closed in SOLID with more interfaces!

        Task<Employee?> RetrieveEmployeeByNameAsync(string employeeName, CancellationToken cancellationToken = default);
    }
}