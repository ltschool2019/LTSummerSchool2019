using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Controller providing basic employee operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeService"></param>
        /// <param name="mapper"></param>
        /// <param name="db"></param>
        public EmployeeController(IEmployeeService employeeService, IMapper mapper, DbContext db) : base(db, mapper)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult> GetEmployees()
        {
            var employees = await _employeeService.GetAllAsync(CurrentEmployeeId);

            return Ok(Mapper.Map<EmployeeDto[]>(employees));
        }

        /// <summary>
        /// Returns employees on project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        /// <response code="200">List of employee on project</response>
        /// <response code="404">Project not found</response>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(EmployeeDto[]), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetEmployeesByProject(int projectId)
        {
            var employees = await _employeeService.GetByProjectIdAsync(CurrentEmployeeId, projectId);

            return Ok(Mapper.Map<EmployeeDto[]>(employees));
        }

        /// <summary>
        /// Sends this user information.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <returns> Basic Employee information </returns>
        /// <response code="200">Basic Employee information</response>
        /// <response code="404">User not found </response>
        [HttpGet("{employeeId}/info")]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        [ProducesResponseType(404)]
        [Authorize(Policy = "AccessAllowed")]
        public async Task<ActionResult> GetInfoAsync(int employeeId)
        {
            var employee = await _employeeService.GetByIdAsync(employeeId);

            return Ok(Mapper.Map<EmployeeDto>(employee));
        }

        /// <summary>
        /// Gets a list of all human leaves.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <returns>User's leave list</returns>
        /// <response code="200">User's leave list</response>
        /// <response code="404">User not found </response>
        [HttpGet("{employeeId}/leaves")]
        [ProducesResponseType(typeof(ICollection<LeaveDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetLeavesAsync(int employeeId)
        {
            var employee = await _employeeService.GetByIdAsync(employeeId);

            return Ok(Mapper.Map<LeaveDto[]>(employee.Leaves));
        }

        /// <summary>
        /// POST api/employee/{id}/leaves
        /// Add new leaves for user.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <param name="leaves">List of LeaveDto that is added to the user</param>
        /// <returns>Was the operation successful?</returns>
        /// <response code="200">Operation successful</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Employee not found</response>
        [HttpPost("{employeeId}/leaves")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> AddLeave(int employeeId, [FromBody] LeaveInputDto leaves)
        {
            if (leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _employeeService.AddLeaveAsync(employeeId, Mapper.Map<Leave>(leaves));
            return Ok(Mapper.Map<LeaveDto>(entity));
        }

        /// <summary>
        /// PUT api/employee/{id}/leaves
        /// Updates information on leaves.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <param name="leaves">List of leaves that updated</param>
        /// <returns>Was the operation successful?</returns>
        /// <response code="200">Operation successful</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Employee not found</response>
        [HttpPut("{employeeId}/leaves")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateLeave(int employeeId, [FromBody] LeaveDto leave)
        {
            if (leave == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _employeeService.UpdateLeaveAsync(employeeId, Mapper.Map<Leave>(leave));
            return Ok(Mapper.Map<LeaveDto>(entity));
        }

        /// <summary>
        /// DELETE api/employee/{id}/leaves?leaveID=1&leaveID=2&leaveID=3
        /// Deletes a leaves record.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <param name="leaveIds"> Ids of leaves that should be deleted</param>
        /// <returns>Was the operation successful?</returns>
        /// <response code="200">Operation successful</response>
        /// <response code="400">Bad request</response>
        /// <response code="403">You cannot change another employee’s leave</response>
        /// <response code="404">Employee not found</response>
        [HttpDelete("{employeeId}/leaves")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteLeave(int employeeId, [FromQuery] int leaveId)
        {
            await _employeeService.DeleteLeaveAsync(employeeId, leaveId);

            return Ok();
        }
    }
}
