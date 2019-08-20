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
        private readonly HttpContext _httpContext;

        /// <summary>
        /// The method returns true if the user tries to change his data or he is a manager or administrator.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Is it possible to change the data</returns>
        private async Task<bool> AccessAllowed(int id)
        {
            var employeeIdFromClaim = _httpContext.User.FindFirstValue("EmployeeID");//We are looking for EmployeeID.
            var authorizedUser =
                await DbContext.Set<Employee>().SingleOrDefaultAsync(
                    e => e.Id == Convert.ToInt32(employeeIdFromClaim)); //We load Employee table.
            var maxRole = authorizedUser.MaxRole;

            return authorizedUser.Id == id ||
                   maxRole == RoleType.Manager ||
                   maxRole == RoleType.Administrator;
        }
        public TaskService(DbContext db, IMapper mapper, HttpContext httpContext) : base(db, mapper)
        {
            _httpContext = httpContext;
        }

        public async Task<Response<List<Task>>> GetTasksAsync(int projectId, int employeeId, DateTime startDate, DateTime endDate)
        {
            //if (!this.AccessAllowed(employeeId).Result)
            //{
            //    return new Response<List<Task>>(HttpStatusCode.BadRequest, $"User not allowed to change data for employee with {employeeId}.");
            //}

            var tasks = await DbContext.Set<TaskNote>()
                .Include(tn => tn.Task)
                .Where(tn => tn.Task.EmployeeId == employeeId && tn.Task.ProjectId == projectId
                                                              && tn.Day >= startDate && tn.Day <= endDate)
                .Select(tn => new Task()
                {
                    Id = tn.TaskId,
                    Name = tn.Task.Name,
                    ProjectId = tn.Task.ProjectId,
                    EmployeeId = tn.Task.EmployeeId,
                    TaskNotes = tn.Task.TaskNotes
                }).ToListAsync();


            if (!tasks.Any()) return new Response<List<Task>>(HttpStatusCode.NotFound, "Tasks not found");

            return new Response<List<Task>>(tasks);

            //var intersectingEmployeeLeave = await DbContext.Set<Leave>()
            //    .Join(DbContext.Set<Employee>(), l => l.EmployeeId, e => e.Id, (l, e) => new { l, e })
            //    .Where(w => w.l.EmployeeId == employeeId && endDate >= w.l.StartDate && startDate <= w.l.EndDate)
            //    .ToListAsync();

            //List<LeaveDto> leave = new List<LeaveDto>();
            //foreach (var item in intersectingEmployeeLeave)
            //{
            //    var iStart = item.l.StartDate < startDate ? startDate : item.l.StartDate;
            //    var iEnd = item.l.EndDate < endDate ? item.l.EndDate : endDate;
            //    leave.Add(new LeaveDto { StartDate = iStart, EndDate = iEnd, Id = item.l.Id, TypeLeave = (TypeLeaveDto)item.l.TypeLeave });
            //}
            //var employeeTaskProject = DbContext.Set<Task>().FirstOrDefault(t => t.ProjectId == projectId && t.EmployeeId == employeeId);
            //if (employeeTaskProject != null)
            //{
            //    List<TaskNoteDto> taskNotes = new List<TaskNoteDto>();
            //    var notes = await DbContext.Set<TaskNote>().Where(tn => tn.TaskId == employeeTaskProject.Id && tn.Day <= endDate && tn.Day >= startDate).ToListAsync();
            //    foreach (var item in notes)
            //        taskNotes.Add(new TaskNoteDto { Day = item.Day, Hours = item.Hours, Id = item.Id });
            //    List<TaskDto> result = new List<TaskDto>();
            //    result.Add(new TaskDto { Name = employeeTaskProject.Name, Leave = leave, TaskNotes = taskNotes, Id = employeeTaskProject.Id });
            //    return new Response<List<Task>>(result);
            //}
            //return new Response<List<Task>>(HttpStatusCode.NotFound, "NotFound");
        }

        public async Task<Response<Task>> AddTaskAsync(int projectId, int employeeId, Task task)
        {
            //if (!AccessAllowed(employeeId).Result)
            //{
            //    return new Response<Task>(HttpStatusCode.BadRequest, $"User not allowed to change data for employee with {employeeId}.");
            //}

            var templateTypeProject = DbContext.Set<Project>().FirstOrDefault(p => p.TemplateType == TemplateType.HoursPerProject && p.Id == projectId);
            var employeeProject = DbContext.Set<ProjectEmployee>().FirstOrDefault(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);
            var nameTask = DbContext.Set<Task>().FirstOrDefault(t => (t.Name == task.Name || t.Name == templateTypeProject.Name) && t.ProjectId == projectId && t.EmployeeId == employeeId);
            if (nameTask == null && templateTypeProject != null && task != null && templateTypeProject.Name == task.Name && employeeProject != null)
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
                        return new Response<Task>(HttpStatusCode.OK, "Ok");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return new Response<Task>(HttpStatusCode.BadRequest, "BadRequest");
        }

        public async Task<Response<Task>> UpdateTaskAsync(int employeeId, Task task)
        {
            if (!this.AccessAllowed(employeeId).Result)
            {
                return new Response<Task>(HttpStatusCode.BadRequest, $"User not allowed to change data for employee with {employeeId}.");
            }

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
                return new Response<Domain.Entities.Task>(HttpStatusCode.OK, "Ok");
            }
            return new Response<Domain.Entities.Task>(HttpStatusCode.NotFound, "Not Found");
        }
        public async Task<Response<Domain.Entities.Task>> DeleteTaskAsync(int taskId, int employeeId)
        {
            if (!this.AccessAllowed(employeeId).Result)
            {
                return new Response<Domain.Entities.Task>(HttpStatusCode.BadRequest, $"User not allowed to change data for employee with {employeeId}.");
            }
            var task = DbContext.Set<Domain.Entities.Task>().Where(t => t.Id == taskId).FirstOrDefault();
            if (task != null)
            {
                DbContext.Set<Domain.Entities.Task>().Remove(task);
                await DbContext.SaveChangesAsync();
                return new Response<Domain.Entities.Task>(HttpStatusCode.OK, "Ok");
            }
            else
            {
                return new Response<Domain.Entities.Task>(HttpStatusCode.NotFound, "Not Found");
            }
        }
    }
}
