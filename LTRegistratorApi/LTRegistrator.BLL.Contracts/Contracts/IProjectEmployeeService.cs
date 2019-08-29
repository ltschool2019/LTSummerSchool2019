using LTRegistrator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IProjectEmployeeService
    {
        Task<Response<ProjectEmployee>> GetEmployeeIdAndProjectIdAsync(int employeeId, int projectId);
    }
}
