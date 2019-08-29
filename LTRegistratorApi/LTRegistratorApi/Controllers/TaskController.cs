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
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, ILeaveService leaveService, IEmployeeService employeeService, IProjectService projectService, IMapper mapper)
        {
            _taskService = taskService;
            _leaveService = leaveService;
            _employeeService = employeeService;
            _projectService = projectService;
            _mapper = mapper;
        }

        /// <summary>
        /// POST api/task/project/{projectId}/employee/{EmployeeId}
        /// Adding project tasks
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <param name="employeeId">id of employee</param>
        /// <param name="task">json {Name, List<{Day, Hours}></param>
        /// <returns>"200 ok" or "400 Bad Request" or "401 Unauthorized"</returns>
        [HttpPost("project/{projectId}/employee/{EmployeeId}")]
        public async Task<ActionResult> AddTask([FromRoute] int projectId, int employeeId, [FromBody] TaskInputDto task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _taskService.AddTaskAsync(projectId, employeeId, _mapper.Map<LTRegistrator.Domain.Entities.Task>(task));
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
        [HttpGet("project/{projectId}/employee/{employeeId}")]
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
                StatusCode((int)employeeResponse.Error.StatusCode, employeeResponse.Error.Message);
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