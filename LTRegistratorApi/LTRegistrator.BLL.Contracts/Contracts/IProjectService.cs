using System.Threading.Tasks;
using LTRegistrator.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IProjectService
    {
        Task<Project> GetByIdAsync(int authUserId, int projectId);
        Task<Project> AddAsync(int authUserId, Project project);
        Task UpdateAsync(int authEmployeeId, Project project);
    }
}