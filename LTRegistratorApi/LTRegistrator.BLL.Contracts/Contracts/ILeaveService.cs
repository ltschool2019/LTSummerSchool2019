using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTRegistrator.Domain.Entities;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface ILeaveService
    {
        Task<ICollection<Leave>> GetLeavesByEmployeeIdAsync(int employeeId, DateTime startDate, DateTime endDate);
    }
}