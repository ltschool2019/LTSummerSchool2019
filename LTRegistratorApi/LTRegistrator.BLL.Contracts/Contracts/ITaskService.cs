using System;
using System.Threading.Tasks;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface ITaskService
    {
        Task<Domain.Entities.Task[]> GetAllByEmployeeIdAndProjectId(int authUserId, int employeeId, int projectId, DateTime startDate, DateTime endDate);
        Task<Domain.Entities.Task> GetByIdAsync(int authUserId, int taskId);
        Task AddAsync(Domain.Entities.Task task);
        Task UpdateAsync(Domain.Entities.Task task);
        Task RemoveAsync(int authUserId, int taskId);
    }
}