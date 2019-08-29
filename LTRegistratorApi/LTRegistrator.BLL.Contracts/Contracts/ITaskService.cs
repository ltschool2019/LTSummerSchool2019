using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTRegistrator.Domain.Entities;
using Task = LTRegistrator.Domain.Entities.Task;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface ITaskService
    {
        Task<Response<List<Task>>> GetTasksAsync(int projectId, int employeeId, DateTime startDate, DateTime endDate);
        Task<Response<Task>> AddTaskAsync(int projectId, int employeeId, Project templateType, Task task);
        Task<Response<Task>> UpdateTaskAsync(int employeeId, Task task);
        Task<Response<Task>> DeleteTaskAsync(int taskId, int employeeId);
    }
}