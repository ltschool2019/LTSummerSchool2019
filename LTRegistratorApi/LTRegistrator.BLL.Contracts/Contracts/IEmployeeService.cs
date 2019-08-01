using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTRegistrator.BLL.Contracts.Dtos;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IEmployeeService
    {
        Task<Response<EmployeeDto>> GetByIdAsync(int id);
        Task<Response<EmployeeDto>> AddLeavesAsync(int userId, ICollection<LeaveDto> leaves);
        Task<Response<EmployeeDto>> UpdateLeavesAsync(int userId, ICollection<LeaveDto> leaves);
        Task<Response<EmployeeDto>> DeleteLeavesAsync(int userId, ICollection<int> leaveIds);
    }
}