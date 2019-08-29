using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task = LTRegistrator.Domain.Entities.Task;

namespace LTRegistrator.BLL.Services.Services
{
    public class TaskService : BaseService, ITaskService
    {
        public TaskService(DbContext db, IMapper mapper) : base(db, mapper){}
        public async Task<Response<List<Task>>> GetTasksAsync(int projectId, int employeeId, DateTime startDate, DateTime endDate)
        {         
            var tasks = await DbContext.Set<TaskNote>()
                 .Include(tn => tn.Task)
                 .Where(tn => tn.Task.EmployeeId == employeeId && tn.Task.ProjectId == projectId)
                 .Select(tn => new Task()
                 {
                     Id = tn.TaskId,
                     Name = tn.Task.Name,
                     ProjectId = tn.Task.ProjectId,
                     EmployeeId = tn.Task.EmployeeId,
                     TaskNotes = tn.Task.TaskNotes.Where(n => n.Day >= startDate && n.Day <= endDate && n.TaskId == n.Task.Id).ToList()                    
                 }).ToListAsync();
            if (tasks == null)
            {
                return new Response<List<Task>>(HttpStatusCode.NotFound, "Tasks not found");
            }
            return new Response<List<Task>>(tasks);
        }

        public async Task<Response<Task>> AddTaskAsync(int projectId, int employeeId, Project templateTypeProject, Task task)
        {                        
            var nameTask = DbContext.Set<Task>().FirstOrDefault(t => (t.Name == task.Name || t.Name == templateTypeProject.Name) && t.ProjectId == projectId && t.EmployeeId == employeeId);
            if (nameTask == null  && templateTypeProject.Name == task.Name )
            {
                using (var transaction = DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        Task newTask = new Task
                        {
                            EmployeeId = employeeId,
                            ProjectId = projectId,
                            Name = task.Name
                        };
                        DbContext.Set<Task>().Add(newTask);

                        foreach (var item in task.TaskNotes)
                        {
                            TaskNote taskNote = new TaskNote
                            {
                                TaskId = newTask.Id,
                                Day = item.Day,
                                Hours = item.Hours
                            };
                            DbContext.Set<TaskNote>().Add(taskNote);
                        }
                        await DbContext.SaveChangesAsync();
                        transaction.Commit();
                        return new Response<Task>(newTask);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return new Response<Task>(HttpStatusCode.BadRequest, "Task is not correct");
        }

        public async Task<Response<Task>> UpdateTaskAsync(int employeeId, Task task)
        {
            var temp = DbContext.Set<Task>().SingleOrDefault(t => t.Id == task.Id && t.Name == task.Name);
            if (temp != null)
            {
                foreach (var item in task.TaskNotes)
                {
                    var note = DbContext.Set<TaskNote>().Where(tn => tn.Day == item.Day && tn.TaskId == task.Id).FirstOrDefault();
                    if (note != null && note.Hours != item.Hours)
                    {
                        note.Hours = item.Hours;
                        DbContext.Set<TaskNote>().Update(note);
                        await DbContext.SaveChangesAsync();
                    }
                    if (note == null)
                    {
                        TaskNote taskNote = new TaskNote
                        {
                            TaskId = task.Id,
                            Day = item.Day,
                            Hours = item.Hours
                        };
                        DbContext.Set<TaskNote>().Add(taskNote);
                        await DbContext.SaveChangesAsync();
                    }
                }
                return new Response<Task>(HttpStatusCode.OK, "Ok");
            }
            return new Response<Task>(HttpStatusCode.NotFound, "Not Found");
        }
        public async Task<Response<Task>> DeleteTaskAsync(int taskId, int employeeId)
        {
            var task = DbContext.Set<Task>().Where(t => t.Id == taskId).FirstOrDefault();
            if (task != null)
            {
                DbContext.Set<Task>().Remove(task);
                await DbContext.SaveChangesAsync();
                return new Response<Task>(HttpStatusCode.OK, "Ok");
            }
            else
            {
                return new Response<Task>(HttpStatusCode.NotFound, "Not Found");
            }
        }
    }
}
