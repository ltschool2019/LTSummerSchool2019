using System.Threading.Tasks;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface ITaskService
    {
        Task<Domain.Entities.Task> GetByIdAsync(int authUserId, int taskId);
    }
}