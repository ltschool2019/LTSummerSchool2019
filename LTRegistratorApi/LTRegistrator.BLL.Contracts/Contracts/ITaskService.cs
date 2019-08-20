using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTRegistrator.Domain.Entities;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface ITaskService
    {
        //Task<Response<List<Domain.Entities.Task>>> GetTasksAsync(int projectId, int employeeId,  DateTime startDate, DateTime endDate);
        Task<Response<Domain.Entities.Task>> AddTaskAsync(int projectId, int employeeId, Domain.Entities.Task task);
        Task<Response<Domain.Entities.Task>> UpdateTaskAsync(int employeeId, Domain.Entities.Task task);
        Task<Response<Domain.Entities.Task>> DeleteTaskAsync(int taskId, int employeeId);
    }
}