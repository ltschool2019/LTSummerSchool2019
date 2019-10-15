using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTRegistrator.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IEmployeeService
    {
        Task<Employee[]> GetAllAsync(int authEmployeeId);
        Task<Employee[]> GetByProjectIdAsync(int authUserId, int projectId);
        Task<Employee> GetByIdAsync(int id);
        Task<Leave> AddLeaveAsync(int userId, Leave leave);
        Task<Leave> UpdateLeaveAsync(int userId, Leave leave);
        Task DeleteLeaveAsync(int userId, int leaveId);
    }
}