using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class TaskController : Controller
    {
        /// <summary>
        /// POST api/task/project/{Id}
        /// Adding project tasks
        /// </summary>
        [HttpPost("project/{Id}")]
        public async Task<ActionResult> AddTask(int id, [FromBody] ICollection<TaskInputDto> task)
        {
            if (task == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var thisUser = await _userManager.FindByIdAsync(
                _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _employeeService.AddLeavesAsync(id, _mapper.Map<ICollection<Leave>>(leaves));
            return response.Status == ResponseResult.Success ? Ok("Leaves have been added") : StatusCode((int)response.Error.StatusCode, new { Message = response.Error.Message });
        }
    }
}