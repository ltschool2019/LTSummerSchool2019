using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Model;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Controller providing basic employee operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[ApiController, Authorize]
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
        /// GET api/employee/info
        /// Sends this user information.
        /// </summary>
        /// <returns>Basic Employee information</returns>
        [HttpGet("{id}/info")]
        public async Task<ActionResult> GetInfoAsync(int id)
        {
            var response = await _employeeService.GetByIdAsync(id);
            
            return response.Status == ResponseResult.Success 
            ? Ok(_mapper.Map<EmployeeDto>(response.Result)) 
            : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }

        /// <summary>
        /// GET api/employee/{id}/leaves
        /// Gets a list of all human leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>User's leave list</returns>
        [HttpGet("{id}/leaves")]
        public async Task<ActionResult> GetLeavesAsync(int id)
        {
            var response = await _employeeService.GetByIdAsync(id);

            return response.Status == ResponseResult.Success 
                ? Ok(_mapper.Map<ICollection<LeaveDto>>(response.Result.Leaves)) 
                : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }

        /// <summary>
        /// POST api/employee/{id}/leaves
        /// Add new leaves for user.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of LeaveDto that is added to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPost("{id}/leaves")]
        public async Task<ActionResult> SetLeavesAsync(int id, [FromBody] ICollection<LeaveInputDto> leaves)
        {
            if (leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.AddLeavesAsync(id, _mapper.Map<ICollection<Leave>>(leaves));
            return response.Status == ResponseResult.Success ? (ActionResult)Ok() : StatusCode((int)response.Error.StatusCode, new { Message = response.Error.Message});
        }

        /// <summary>
        /// PUT api/employee/{id}/leaves
        /// Updates information on leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that updated</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPut("{id}/leaves")]
        public async Task<ActionResult> UpdateLeaves(int id, [FromBody] List<LeaveDto> leaves)
        {
            if (leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.UpdateLeavesAsync(id, _mapper.Map<ICollection<Leave>>(leaves));
            return response.Status == ResponseResult.Success ? (ActionResult)Ok() : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }

        /// <summary>
        /// DELETE api/employee/{id}/leaves?leaveID=1&leaveID=2&leaveID=3
        /// Deletes a leaves record.
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="id"> IDs of leaves that should be deleted</param>
        /// <returns>Was the operation successful?</returns>
        [HttpDelete("{userId}/leaves")]
        public async Task<ActionResult> DeleteLeave(int userId, [FromQuery] List<int> leaveID)
        {
            if ( leaveID== null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.DeleteLeavesAsync(userId, leaveID);
            return response.Status == ResponseResult.Success ? (ActionResult)Ok() : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }
    }
}
