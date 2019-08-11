using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using LTRegistrator.BLL.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LTRegistrator.BLL.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Сontroller providing operations with tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class TaskController : ControllerBase
    {
        private readonly LTRegistratorDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly HttpContext _httpContext;

        public TaskController(LTRegistratorDbContext context, UserManager<User> userManager, HttpContext httpContext)
        {
            _httpContext = httpContext;
            _db = context;
            _userManager = userManager;
        }
        /// <summary>
        /// POST api/task/project/{projectId}
        /// Adding project tasks
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <param name="task">json {Name, List<{Day, Hours}></param>
        /// <returns>"200 ok" or "400 Bad Request" or "401 Unauthorized"</returns>
        [HttpPost("project/{projectId}")]
        public async Task<ActionResult> AddTask([FromRoute] int projectId, [FromBody] TaskInputDto task)
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (thisUser == null)
            {
                return BadRequest();
            }           
            var authorizedUser =
                await _db.Set<Employee>().SingleOrDefaultAsync(
                    e => e.Id == thisUser.EmployeeId);
            var templateTypeProject = _db.Project.Where(p => p.TemplateType == TemplateType.HoursPerProject && p.Id == projectId).FirstOrDefault();
            var employeeProject = _db.ProjectEmployee.Where(pe => pe.ProjectId == projectId && pe.EmployeeId == thisUser.EmployeeId).FirstOrDefault();
            var nameTask = _db.Task.Where(t => (t.Name == task.Name || t.Name == templateTypeProject.Name)  && t.ProjectId == projectId && t.EmployeeId == thisUser.EmployeeId).FirstOrDefault(); 
            if (nameTask == null && templateTypeProject != null && task != null && templateTypeProject.Name == task.Name && employeeProject != null)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        LTRegistrator.Domain.Entities.Task newTask = new LTRegistrator.Domain.Entities.Task
                        {
                            EmployeeId = authorizedUser.Id,
                            ProjectId = projectId,
                            Name = task.Name
                        };
                        _db.Task.Add(newTask);
                        
                        foreach (var item in task.TaskNotes)
                        {                          
                                TaskNote taskNote = new TaskNote
                                {
                                    TaskId = newTask.Id,
                                    Day = item.Day,
                                    Hours = item.Hours
                                };
                                _db.TaskNote.Add(taskNote);                          
                        }
                        await _db.SaveChangesAsync();
                        transaction.Commit();
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return BadRequest();      
        }
        /// <summary>
        /// GET api/task/project/{ProjectId}/from/{StartDate}/to/{EndDate}
        /// Output information on tasks for a certain period of time
        /// </summary>
        /// <param name="ProjectId">id of project</param>
        /// <param name="EmployeeId">id of project</param>
        /// <param name="StartDate">period start date</param>
        /// <param name="EndDate">period end date</param>
        /// <returns>Task information list</returns>
        [HttpGet("project/{ProjectId}/employee/{employeeId}")]
        public async Task<ActionResult<List<TaskDto>>> GetTask([FromRoute] int ProjectId, int EmployeeId,[FromQuery] DateTime StartDate,[FromQuery] DateTime EndDate)
        {
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (thisUser == null)
            {
                return BadRequest();
            }

            if (!this.AccessAllowed(EmployeeId).Result)
            {
                return BadRequest();
            }
            var authorizedUser =
                await _db.Set<Employee>().SingleOrDefaultAsync(
                    e => e.Id == EmployeeId);
            var intersectingEmployeeLeave = await _db.Leave.Join(_db.Employee,
                                                        l => l.EmployeeId,
                                                        e => e.Id,
                                                        (l, e) => new { l, e }).Where(w => w.l.EmployeeId == EmployeeId && EndDate >= w.l.StartDate && StartDate <= w.l.EndDate).ToListAsync();
            List<LeaveDto> leave = new List<LeaveDto>();
            foreach (var item in intersectingEmployeeLeave)
            {
                var iStart = item.l.StartDate < StartDate ? StartDate : item.l.StartDate;
                var iEnd = item.l.EndDate < EndDate ? item.l.EndDate : EndDate;
                leave.Add(new LeaveDto { StartDate = iStart, EndDate = iEnd, Id = item.l.Id});            
            }
            var employeeTaskProject = _db.Task.Where(t => t.ProjectId == ProjectId && t.EmployeeId == thisUser.EmployeeId).FirstOrDefault();
            if (employeeTaskProject != null)
            {             
                List<TaskNoteDto> taskNotes = new List<TaskNoteDto>();
                var notes = await _db.TaskNote.Where(tn => tn.TaskId == employeeTaskProject.Id).ToListAsync();
                foreach (var item in notes)
                    taskNotes.Add(new TaskNoteDto { Day = item.Day, Hours = item.Hours}) ;
                List<TaskDto> result = new List<TaskDto>();
                result.Add(new TaskDto { Name = employeeTaskProject.Name, Leave = leave, TaskNotes = taskNotes });
                return (result);
            }          
            return BadRequest();
        }
        /// <summary>
        /// Updating task information
        /// PUT: api/Task/{TaskId}
        /// </summary>
        /// <param name="TaskId">id of task</param>
        /// <param name="task">json {Name, List<{Day, Hours}></param>
        /// <returns> "OK" or "not found"</returns>
        [HttpPut("{TaskId}")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskInputDto task, [FromRoute] int TaskId)
        {
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (thisUser == null)
            {
                return BadRequest();
            }
            var temp = _db.Task.SingleOrDefault(t => t.Id == TaskId && t.Name == task.Name);
            if (temp != null)
            {
                foreach (var item in task.TaskNotes)
                {
                    var note = _db.TaskNote.Where(tn => tn.Day == item.Day && tn.TaskId == TaskId).FirstOrDefault();
                    if (note != null && note.Hours != item.Hours)
                    {
                        note.Hours = item.Hours;
                        _db.TaskNote.Update(note);
                        await _db.SaveChangesAsync();
                    }
                    if (note == null)
                    {
                        TaskNote taskNote = new TaskNote
                        {
                            TaskId = TaskId,
                            Day = item.Day,
                            Hours = item.Hours
                        };
                        _db.TaskNote.Add(taskNote);
                        await _db.SaveChangesAsync();                       
                    }
                }
                return Ok();
            }
            return NotFound();
        }
        /// <summary>
        /// Method for removing the task from the project
        /// DELETE: api/task/{TaskId}
        /// </summary>
        /// <param name="TaskId"> id of the task to be deleted</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpDelete("{TaskId}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int TaskId)
        {
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (thisUser == null)
            {
                return BadRequest();
            }
            var task = _db.Task.Where(t => t.Id == TaskId).FirstOrDefault();
            if (task != null)
            {
                _db.Task.Remove(task);

                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        /// <summary>
        /// The method returns true if the user tries to change his data or he is a manager or administrator.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Is it possible to change the data</returns>
        private async Task<bool> AccessAllowed(int id)
        {
            var thisUser = await _userManager.FindByIdAsync(
                _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)); //We are looking for an authorized user.
            var authorizedUser =
                await _db.Employee.SingleOrDefaultAsync(
                    e => e.Id ==
                         thisUser.EmployeeId); //We load Employee table.
            var maxRole = authorizedUser.MaxRole;


            return authorizedUser.Id == id ||
                   maxRole == RoleType.Manager ||
                   maxRole == RoleType.Administrator;
        }
    }
}