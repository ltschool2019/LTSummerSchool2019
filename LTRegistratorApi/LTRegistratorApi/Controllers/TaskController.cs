using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Task = LTRegistrator.Domain.Entities.Task;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Сontroller providing operations with tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILeaveService _leaveService;
        private readonly IEmployeeService _employeeService;
        private readonly IProjectEmployeeService _projectEmployeeService;
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, IProjectEmployeeService projectEmployeeService, ILeaveService leaveService, IEmployeeService employeeService, IProjectService projectService, IMapper mapper)
        {
            _taskService = taskService;
            _leaveService = leaveService;
            _employeeService = employeeService;
            _projectService = projectService;
            _projectEmployeeService = projectEmployeeService;
            _mapper = mapper;
        }

        /// <summary>
        /// POST api/task/project/{projectId}/employee/{EmployeeId}
        /// Adding project tasks
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <param name="employeeId">id of employee</param>
        /// <param name="task">json {Name, List<{Day, Hours}></param>
        /// <returns>"200 ok" or "400 Bad Request" or "403 Forbidden"</returns>
        /// <response code="200">Project task added</response>
        /// <response code="400">Bad request</response>
        /// <response code="403">You do not have sufficient permissions to change data for this employee</response>
        [HttpPost("project/{projectId}/employee/{EmployeeId}")]
        [Authorize(Policy = "AccessAllowed")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<ActionResult> AddTask([FromRoute] int projectId, int employeeId, [FromBody] TaskInputDto task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (employeeId == null || projectId == null || task == null)
            {
                return BadRequest();
            }
            var templateTypeProject = await _projectService.GetTemplateTypeByIdAsync(projectId);
            if (templateTypeProject.Status == ResponseResult.Error)
            {
                StatusCode((int)templateTypeProject.Error.StatusCode, templateTypeProject.Error.Message);
            }
            var employeeProject = await _projectEmployeeService.GetEmployeeIdAndProjectIdAsync(employeeId, projectId);
            if (employeeProject.Status == ResponseResult.Error)
            {
                StatusCode((int)employeeProject.Error.StatusCode, employeeProject.Error.Message);
            }
            var response = await _taskService.AddTaskAsync(projectId, employeeId, templateTypeProject.Result, _mapper.Map<Task>(task));
            return response.Status == ResponseResult.Success ? (ActionResult)Ok() : StatusCode((int)response.Error.StatusCode, new { Message = response.Error.Message });
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
        /// <response code="404">Tasks or employee or project not found </response>
        [HttpGet("project/{projectId}/employee/{employeeId}")]
        [ProducesResponseType(typeof(List<TaskDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Authorize(Policy = "AccessAllowed")]
        public async Task<ActionResult> GetTasks([FromRoute] int projectId, int employeeId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest($"Period entered incorrectly. StartDate > EndDate ({startDate} > {endDate})");
            }
            var employeeResponse = await _employeeService.GetByIdAsync(employeeId);
            var projectResponse = await _projectService.GetProjectByIdAsync(projectId);
            var tasksResponse = await _taskService.GetTasksAsync(projectId, employeeId, startDate, endDate);
            var leavesResponse = await _leaveService.GetLeavesByEmployeeIdAsync(employeeId, startDate, endDate);

            foreach (var item in leavesResponse.Result)
            {
                item.StartDate = item.StartDate < startDate ? startDate : item.StartDate;
                item.EndDate = item.EndDate < endDate ? item.EndDate : endDate;
            }
            if (employeeResponse.Status == ResponseResult.Error)
            {
                return StatusCode((int)employeeResponse.Error.StatusCode, new { Message = employeeResponse.Error.Message });
            }
            if (projectResponse.Status == ResponseResult.Error)
            {
                return StatusCode((int)projectResponse.Error.StatusCode, new { Message = projectResponse.Error.Message });
            }
            if (tasksResponse.Status == ResponseResult.Error)
            {
                return StatusCode((int)tasksResponse.Error.StatusCode, new { Message = tasksResponse.Error.Message });
            }          

            var result = _mapper.Map<TaskDto>(tasksResponse.Result.FirstOrDefault());
            _mapper.Map(leavesResponse.Result, result);

            return Ok(result);
        }

        /// <summary>
        /// Updating task information
        /// PUT: api/Task/employee/{employeeId}
        /// </summary>
        /// <param name="employeeId">id of employee</param>
        /// <param name="task">json {Name, List<{Day, Hours}></param>
        /// <returns> "OK" or "not found"</returns>
        [HttpPut("employee/{employeeId}")]
        public async Task<ActionResult> UpdateTask([FromBody] TaskInputDto task, int employeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _taskService.UpdateTaskAsync(employeeId, _mapper.Map<LTRegistrator.Domain.Entities.Task>(task));
            return response.Status == ResponseResult.Success ? (ActionResult)Ok() : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }
        /// <summary>
        /// Method for removing the task from the project
        /// DELETE: api/task/{taskId}/employee/{employeeId}
        /// </summary>
        /// <param name="taskId"> id of the task to be deleted</param>
        /// <param name="employeeId">id of employee</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpDelete("{taskId}/employee/{employeeId}")]
        public async Task<ActionResult> DeleteTask([FromRoute] int taskId, int employeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _taskService.DeleteTaskAsync(taskId, employeeId);
            return response.Status == ResponseResult.Success ? (ActionResult)Ok() : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }
    }
}