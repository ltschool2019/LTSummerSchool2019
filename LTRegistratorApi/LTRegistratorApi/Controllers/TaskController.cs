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
        
        /// <summary> </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="httpContext"></param>
        public TaskController(LTRegistratorDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// POST api/task/project/{projectId}/employee/{EmployeeId}
        /// Adding project tasks
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <param name="employeeId">id of employee</param>
        /// <param name="task">json {Name, List {Day, Hours} </param>
        /// <returns>"200 ok" or "400 Bad Request" or "403 Forbidden"</returns>
        /// <response code="200">Project task added</response>
        /// <response code="400">Bad request</response>
        /// <response code="403">You do not have sufficient permissions to change data for this employee</response>
        [HttpPost("project/{projectId}/employee/{EmployeeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Authorize(Policy = "AccessAllowed")]
        public async Task<ActionResult> AddTask([FromRoute] int projectId, int employeeId, [FromBody] TaskInputDto task)
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var templateTypeProject = _db.Project.FirstOrDefault(p => p.TemplateType == TemplateType.HoursPerProject && p.Id == projectId && !p.SoftDeleted);
            var employeeProject = _db.ProjectEmployee.Where(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId).FirstOrDefault();
            var nameTask = _db.Task.Where(t => (t.Name == task.Name || t.Name == templateTypeProject.Name)  && t.ProjectId == projectId && t.EmployeeId == employeeId).FirstOrDefault(); 
            if (nameTask == null && templateTypeProject != null && task != null && templateTypeProject.Name == task.Name && employeeProject != null)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        LTRegistrator.Domain.Entities.Task newTask = new LTRegistrator.Domain.Entities.Task
                        {
                            EmployeeId = employeeId,
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
        /// GET api/task/project/{projectId}/employee/{employeeId}?StartDate={startDate}&EndDate={endDate}
        /// Output information on tasks for a certain period of time
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <param name="employeeId">id of employee</param>
        /// <param name="startDate">period start date</param>
        /// <param name="endDate">period end date</param>
        /// <returns>Task information list</returns>
        /// <response code="200">Task information list</response>
        /// <response code="403">You do not have sufficient permissions to change data for this employee</response>
        /// <response code="404">Tasks not found</response>
        [HttpGet("project/{projectId}/employee/{employeeId}")]
        [ProducesResponseType(typeof(List<TaskDto>), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Authorize(Policy = "AccessAllowed")]
        public async Task<ActionResult<List<TaskDto>>> GetTasks([FromRoute] int projectId, int employeeId,[FromQuery] DateTime startDate,[FromQuery] DateTime endDate)
        {
            var intersectingEmployeeLeave = await _db.Leave.Join(_db.Employee,
                                                        l => l.EmployeeId,
                                                        e => e.Id,
                                                        (l, e) => new { l, e }).Where(w => w.l.EmployeeId == employeeId && endDate >= w.l.StartDate && startDate <= w.l.EndDate).ToListAsync();
            List<LeaveDto> leave = new List<LeaveDto>();
            foreach (var item in intersectingEmployeeLeave)
            {
                var iStart = item.l.StartDate < startDate ? startDate : item.l.StartDate;
                var iEnd = item.l.EndDate < endDate ? item.l.EndDate : endDate;
                leave.Add(new LeaveDto { StartDate = iStart, EndDate = iEnd, Id = item.l.Id, TypeLeave = (TypeLeaveDto)item.l.TypeLeave});            
            }
            var employeeTaskProject = _db.Task.FirstOrDefault(t => t.ProjectId == projectId && t.EmployeeId == employeeId && !t.ProjectEmployee.Project.SoftDeleted);
            if (employeeTaskProject != null)
            {             
                List<TaskNoteDto> taskNotes = new List<TaskNoteDto>();
                var notes = await _db.TaskNote.Where(tn => tn.TaskId == employeeTaskProject.Id && tn.Day <= endDate && tn.Day>=startDate).ToListAsync();
                foreach (var item in notes)
                    taskNotes.Add(new TaskNoteDto { Day = item.Day, Hours = item.Hours, Id = item.Id}) ;
                List<TaskDto> result = new List<TaskDto>();
                result.Add(new TaskDto { Name = employeeTaskProject.Name, Leave = leave, TaskNotes = taskNotes, Id = employeeTaskProject.Id});
                return Ok(result);
            }          
            return NotFound();
        }
        /// <summary>
        /// Updating task information
        /// PUT: api/Task/employee/{employeeId}
        /// </summary>
        /// <param name="employeeId">id of employee</param>
        /// <param name="task">json {Name, List {Day, Hours} </param>
        /// <returns> "OK" or "not found"</returns>
        /// <response code="200">Task updated</response>
        /// <response code="403">You do not have sufficient permissions to change data for this employee</response>
        /// <response code="404">Task not found</response>
        [HttpPut("employee/{employeeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Authorize(Policy = "AccessAllowed")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskInputDto task, int employeeId)
        {
            var temp = _db.Task.SingleOrDefault(t => t.Id == task.Id && t.Name == task.Name);
            if (temp != null)
            {
                foreach (var item in task.TaskNotes)
                {
                    var note = _db.TaskNote.FirstOrDefault(tn => tn.Day == item.Day && tn.TaskId == task.Id);
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
                            TaskId = task.Id,
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
        /// DELETE: api/task/{taskId}/employee/{employeeId}
        /// </summary>
        /// <param name="taskId"> id of the task to be deleted</param>
        /// <param name="employeeId">id of employee</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        /// <response code="200">Task deleted</response>
        /// <response code="403">You do not have sufficient permissions to change data for this employee</response>
        /// <response code="404">Task not found</response>
        [HttpDelete("{TaskId}/employee/{employeeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Authorize(Policy = "AccessAllowed")]
        public async Task<IActionResult> DeleteTask([FromRoute] int taskId, int employeeId)
        {
            var task = _db.Task.Where(t => t.Id == taskId).FirstOrDefault();
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
    }
}