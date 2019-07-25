using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Dtos;
using LTRegistratorApi.Model.ResourceModels;
using LTRegistratorApi.Validators;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Controller providing basic employee operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        public EmployeeController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        /// <summary>
        /// GET api/employee/{id}/info
        /// Sends user information.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>Basic Employee information</returns>
        [HttpGet("{id}/info")]
        public async Task<ActionResult> GetInfoAsync(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound($"Employee with Id = {id} not found");
            }

            return Ok(_mapper.Map<EmployeeResourceModel>(employee));
        }

        /// <summary>
        /// GET api/employee/{id}/leaves
        /// Gets a list of all human leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>User's leave list</returns>
        [HttpGet("{id}/leaves")]
        public async Task<ActionResult> GetLeavesAsync(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound($"Employee with Id = {id} not found");
            }

            return Ok(_mapper.Map<ICollection<LeaveResourceModel>>(employee.Leave));
        }

        /// <summary>
        /// POST api/employee/{id}/leaves
        /// Add new leaves for user.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of LeaveDto that is added to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPost("{id}/leaves")]
        public async Task<ActionResult> SetLeavesAsync(Guid id, [FromBody] ICollection<LeaveResourceModel> leaves)
        {
            if (id == Guid.Empty || leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.AddLeavesAsync(id, _mapper.Map<ICollection<EmployeeLeaveDto>>(leaves));
            return response.Status == ResponseResult.Success ? Ok("Leaves have been added") : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }

        /// <summary>
        /// PUT api/employee/{id}/leaves
        /// Updates information on leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that updated</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPut("{id}/leaves")]
        public async Task<ActionResult> UpdateLeaves(Guid id, [FromBody] List<LeaveResourceModel> leaves)
        {
            if (id == Guid.Empty || leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.UpdateLeavesAsync(id, _mapper.Map<ICollection<LeaveDto>>(leaves));
            return response.Status == ResponseResult.Success ? Ok("Leaves have been updated") : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }

        /// <summary>
        /// DELETE api/employee/{id}/leaves
        /// Deletes a leaves record.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that is deleted to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpDelete("{id}/leaves")]
        public async Task<ActionResult> DeleteLeave(Guid id, [FromBody] List<LeaveResourceModel> leaves)
        {
            if (id == Guid.Empty || leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.DeleteLeavesAsync(id, _mapper.Map<ICollection<LeaveDto>>(leaves));
            return response.Status == ResponseResult.Success ? Ok("Leaves have been deleted") : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }
    }
}
