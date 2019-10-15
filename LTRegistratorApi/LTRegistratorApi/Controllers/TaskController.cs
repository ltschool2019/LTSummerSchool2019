using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Exceptions;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Filters;
using LTRegistratorApi.Model.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Сontroller providing operations with tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController, Authorize, GlobalApiException]
    public class TaskController : BaseController
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="taskService"></param>
        public TaskController(DbContext db, ITaskService taskService, IMapper mapper) : base(db, mapper)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Get task by id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        /// <response code="200">Returns task</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        [HttpGet("{taskId}")]
        [ProducesResponseType(typeof(TaskDto), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetTaskById([FromRoute] int taskId)
        {
            var task = await _taskService.GetByIdAsync(CurrentEmployeeId, taskId);

            return Ok(Mapper.Map<TaskDto>(task));
        }

        /// <summary>
        /// Add new task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <response code="200">Task was created</response>
        /// <response code="409">Task already exist</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(409)]
        public async Task<ActionResult> AddTask(TaskDto task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = Mapper.Map<LTRegistrator.Domain.Entities.Task>(task, opt => opt.Items["EmployeeId"] = CurrentEmployeeId);
            await _taskService.AddAsync(entity);
            return Ok();
        }

        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <response code="200">Task was updated</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateTask(TaskDto task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = Mapper.Map<LTRegistrator.Domain.Entities.Task>(task, opt => opt.Items["EmployeeId"] = CurrentEmployeeId);
            await _taskService.UpdateAsync(entity);
            return Ok();
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
        public async Task<ActionResult> GetTasks([FromRoute] int projectId, int employeeId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (!Db.Set<Project>().Any(p => p.Id == projectId && !p.SoftDeleted))
            {
                throw new NotFoundException("Project not found");
            }
            var intersectingEmployeeLeave = await Db.Set<Leave>().Join(Db.Set<Employee>(),
                                                        l => l.EmployeeId,
                                                        e => e.Id,
                                                        (l, e) => new { l, e }).Where(w => w.l.EmployeeId == employeeId && endDate >= w.l.StartDate && startDate <= w.l.EndDate).ToListAsync();
            List<LeaveDto> leave = new List<LeaveDto>();
            foreach (var item in intersectingEmployeeLeave)
            {
                var iStart = item.l.StartDate < startDate ? startDate : item.l.StartDate;
                var iEnd = item.l.EndDate < endDate ? item.l.EndDate : endDate;
                leave.Add(new LeaveDto { StartDate = iStart, EndDate = iEnd, Id = item.l.Id, TypeLeave = (TypeLeaveDto)item.l.TypeLeave });
            }

            var employeeTaskProject = await Db.Set<LTRegistrator.Domain.Entities.Task>().Where(t => t.ProjectId == projectId && t.EmployeeId == employeeId && !t.ProjectEmployee.Project.SoftDeleted).ToArrayAsync();
            var result = new List<TaskDto>();
            foreach (var task in employeeTaskProject)
            {
                List<TaskNoteDto> taskNotes = new List<TaskNoteDto>();
                var notes = await Db.Set<TaskNote>().Where(tn => tn.TaskId == task.Id && tn.Day <= endDate && tn.Day >= startDate).ToListAsync();
                foreach (var item in notes)
                    taskNotes.Add(new TaskNoteDto { Day = item.Day, Hours = item.Hours, Id = item.Id });
                result.Add(new TaskDto { Name = task.Name, Leave = leave, TaskNotes = taskNotes, Id = task.Id });
            }
            return Ok(result);
            //TODO: refactor
        }

        /// <summary>
        /// Method for removing the task from the project
        /// DELETE: api/task/{taskId}
        /// </summary>
        /// <param name="taskId"> id of the task to be deleted</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        /// <response code="200">Task deleted</response>
        /// <response code="403">You do not have sufficient permissions to change data for this employee</response>
        /// <response code="404">Task not found</response>
        [HttpDelete("{TaskId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteTask([FromRoute] int taskId)
        {
            await _taskService.RemoveAsync(CurrentEmployeeId, taskId);

            return Ok();
        }
    }
}