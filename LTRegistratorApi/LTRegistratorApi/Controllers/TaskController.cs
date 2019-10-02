using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using LTRegistratorApi.Model.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Task = System.Threading.Tasks.Task;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Сontroller providing operations with tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class TaskController : BaseController
    {

        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="taskService"></param>
        public TaskController(DbContext db, ITaskService taskService, IMapper mapper) : base(db)
        {
            _taskService = taskService;
            _mapper = mapper;
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult> GetTaskById([FromRoute] int taskId)
        {
            var task = await _taskService.GetByIdAsync(CurrentEmployeeId, taskId);

            return Ok(_mapper.Map<TaskDto>(task));
        }

        [HttpPost]
        public async Task<ActionResult> AddTask(TaskDto task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var entity = _mapper.Map<LTRegistrator.Domain.Entities.Task>(task, opt => opt.Items["EmployeeId"] = CurrentEmployeeId);
                await _taskService.AddAsync(entity);
            }
            catch (Exception e)
            {
            }
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateTask(TaskDto task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _taskService.UpdateAsync(_mapper.Map<LTRegistrator.Domain.Entities.Task>(task));
            return Ok();
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
        //[HttpPost("project/{projectId}/employee/{EmployeeId}")]
        //[Authorize(Policy = "AccessAllowed")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(403)]
        //public async Task<ActionResult> AddTask([FromRoute] int projectId, int employeeId, [FromBody] TaskInputDto task)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var templateTypeProject = Db.Set<Project>().FirstOrDefault(p => p.TemplateType == TemplateType.HoursPerProject && p.Id == projectId && !p.SoftDeleted);
        //    if (templateTypeProject == null)
        //    {
        //        return NotFound();
        //    }

        //    var employeeProject = Db.Set<ProjectEmployee>().Where(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId).FirstOrDefault();
        //    var nameTask = Db.Set<LTRegistrator.Domain.Entities.Task>().Where(t => (t.Name == task.Name || t.Name == templateTypeProject.Name) && t.ProjectId == projectId && t.EmployeeId == employeeId).FirstOrDefault();
        //    if (nameTask == null && templateTypeProject != null && task != null && templateTypeProject.Name == task.Name && employeeProject != null)
        //    {
        //        using (var transaction = Db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                LTRegistrator.Domain.Entities.Task newTask = new LTRegistrator.Domain.Entities.Task
        //                {
        //                    EmployeeId = employeeId,
        //                    ProjectId = projectId,
        //                    Name = task.Name
        //                };
        //                Db.Set<LTRegistrator.Domain.Entities.Task>().Add(newTask);

        //                foreach (var item in task.TaskNotes)
        //                {
        //                    TaskNote taskNote = new TaskNote
        //                    {
        //                        TaskId = newTask.Id,
        //                        Day = item.Day,
        //                        Hours = item.Hours
        //                    };
        //                    Db.Set<TaskNote>().Add(taskNote);
        //                }
        //                await Db.SaveChangesAsync();
        //                transaction.Commit();
        //                return Ok();
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //            }
        //        }
        //    }
        //    return BadRequest();
        //}

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

            var employeeTaskProject = Db.Set<LTRegistrator.Domain.Entities.Task>().FirstOrDefault(t => t.ProjectId == projectId && t.EmployeeId == employeeId && !t.ProjectEmployee.Project.SoftDeleted);
            if (employeeTaskProject != null)
            {
                List<TaskNoteDto> taskNotes = new List<TaskNoteDto>();
                var notes = await Db.Set<TaskNote>().Where(tn => tn.TaskId == employeeTaskProject.Id && tn.Day <= endDate && tn.Day >= startDate).ToListAsync();
                foreach (var item in notes)
                    taskNotes.Add(new TaskNoteDto { Day = item.Day, Hours = item.Hours, Id = item.Id });
                List<TaskDto> result = new List<TaskDto>
                {
                    new TaskDto { Name = employeeTaskProject.Name, Leave = leave, TaskNotes = taskNotes, Id = employeeTaskProject.Id }
                };
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
        //[HttpPut("employee/{employeeId}")]
        //[Authorize(Policy = "AccessAllowed")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(403)]
        //[ProducesResponseType(404)]
        //public async Task<ActionResult> UpdateTask([FromBody] TaskInputDto task, int employeeId)
        //{
        //    var temp = Db.Set<LTRegistrator.Domain.Entities.Task>().SingleOrDefault(t => t.Id == task.Id && t.Name == task.Name);

        //    if (temp != null)
        //    {
        //        foreach (var item in task.TaskNotes)
        //        {
        //            var note = Db.Set<TaskNote>().FirstOrDefault(tn => tn.Day == item.Day && tn.TaskId == task.Id);
        //            if (note != null && note.Hours != item.Hours)
        //            {
        //                note.Hours = item.Hours;
        //                Db.Set<TaskNote>().Update(note);
        //                await Db.SaveChangesAsync();
        //            }
        //            if (note == null)
        //            {
        //                TaskNote taskNote = new TaskNote
        //                {
        //                    TaskId = task.Id,
        //                    Day = item.Day,
        //                    Hours = item.Hours
        //                };
        //                Db.Set<TaskNote>().Add(taskNote);
        //                await Db.SaveChangesAsync();
        //            }
        //        }
        //        return Ok();
        //    }
        //    return NotFound();
        //}

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
        [Authorize(Policy = "AccessAllowed")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteTask([FromRoute] int taskId, int employeeId)
        {
            var task = Db.Set<LTRegistrator.Domain.Entities.Task>().Where(t => t.Id == taskId).FirstOrDefault();

            if (task != null)
            {
                Db.Set<LTRegistrator.Domain.Entities.Task>().Remove(task);

                await Db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}