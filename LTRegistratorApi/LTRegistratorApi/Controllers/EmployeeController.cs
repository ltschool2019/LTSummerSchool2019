using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Dtos;
using LTRegistratorApi.Model.ResourceModels;
using Microsoft.AspNetCore.Authorization;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Controller providing basic employee operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController, Authorize]
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
            ? Ok(_mapper.Map<EmployeeResourceModel>(response.Result)) 
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
                ? Ok(_mapper.Map<ICollection<LeaveResourceModel>>(response.Result.Leaves)) 
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
        public async Task<ActionResult> SetLeavesAsync(int id, [FromBody] ICollection<LeaveResourceModel> leaves)
        {
            if (leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.AddLeavesAsync(id, _mapper.Map<ICollection<LeaveDto>>(leaves));
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
        public async Task<ActionResult> UpdateLeaves(int id, [FromBody] List<LeaveResourceModel> leaves)
        {
            if (leaves == null)
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
        public async Task<ActionResult> DeleteLeave(int id, [FromBody] List<int> leaves)
        {
            if (leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.DeleteLeavesAsync(id, leaves);
            return response.Status == ResponseResult.Success ? Ok("Leaves have been deleted") : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }
    }
}
