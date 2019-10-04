using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTRegistrator.Domain.Entities;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllAsync(int authEmployeeId);
        Task<Response<Employee>> GetByIdAsync(int id);
        Task<Response<Employee>> AddLeavesAsync(int userId, ICollection<Leave> leaves);
        Task<Response<Employee>> UpdateLeavesAsync(int userId, ICollection<Leave> leaves);
        Task<Response<Employee>> DeleteLeavesAsync(int userId, ICollection<int> leaveIds);
    }
}