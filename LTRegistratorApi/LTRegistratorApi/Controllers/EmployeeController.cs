using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LTRegistratorApi.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Security.Claims;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Controller providing basic employee operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmployeeController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        /// <summary>
        /// GET api/employee/info
        /// Sends this user information.
        /// </summary>
        /// <returns>Basic Employee information</returns>
        [HttpGet("info")]
        public async Task<ActionResult<EmployeeDto>> GetInfoAsync()
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var employee = new List<Employee>() {
                db.Employee
                .Include(e => e.ProjectEmployee)
                    .ThenInclude(pe => pe.Project)
                .SingleOrDefault(e => e.EmployeeId == user.EmployeeId)
            }.First();

            return DtoConverter.ToEmployeeDto(employee);
        }

        /// <summary>
        /// GET api/employee/{id}/leaves
        /// Gets a list of all human leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>User's leave list</returns>
        [HttpGet("{id}/leaves")]
        public async Task<ActionResult<List<Leave>>> GetLeavesAsync(int id)
        {
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var authorizedUser = await db.Employee.SingleOrDefaultAsync(e => e.EmployeeId == thisUser.EmployeeId);
            var maxRole = authorizedUser.MaxRole;

            if (authorizedUser.EmployeeId != id && maxRole != RoleType.Manager && maxRole != RoleType.Administrator)
                return BadRequest();

            var user = await db.Employee
                .Include(e => e.Leaves)
                .SingleOrDefaultAsync(e => e.EmployeeId == id);

            if (user == null)
                return NotFound();

            return user.Leaves.ToList();
        }

        /// <summary>
        /// POST api/employee/{id}/leaves
        /// Add new leaves for user.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leavesDto">List of LeaveDto that is added to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPost("{id}/leaves")]
        public async Task<ActionResult> SetLeavesAsync(int id, [FromBody] List<LeaveDto> leavesDto)
        {
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var authorizedUser = await db.Employee.SingleOrDefaultAsync(e => e.EmployeeId == thisUser.EmployeeId);
            var maxRole = authorizedUser.MaxRole;

            if (authorizedUser.EmployeeId != id && maxRole != RoleType.Manager && maxRole != RoleType.Administrator)
                return BadRequest();

            var user = await db.Employee
                .Include(e => e.Leaves)
                .SingleOrDefaultAsync(V => V.EmployeeId == id);

            if (leavesDto != null && user != null && ModelState.IsValid)
            {
                var leaves = new List<Leave>();
                foreach (var leave in leavesDto)
                    leaves.Add(new Leave { TypeLeave = leave.TypeLeave, StartDate = leave.StartDate, EndDate = leave.EndDate });

                if (LeavesValidator.TryMergLeaves(leaves, user.Leaves.ToList()))
                    foreach (var leave in leaves)
                        user.Leaves.Add(leave);
                else return BadRequest();
            }
            else return NotFound();

            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// PUT api/employee/{id}/leaves
        /// Updates information on leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that updated</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPut("{id}/leaves")]
        public async Task<ActionResult> UpdateLeavesAsync(int id, [FromBody] List<Leave> leaves)
        {
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var authorizedUser = await db.Employee.SingleOrDefaultAsync(e => e.EmployeeId == thisUser.EmployeeId);
            var maxRole = authorizedUser.MaxRole;

            if (authorizedUser.EmployeeId != id && maxRole != RoleType.Manager && maxRole != RoleType.Administrator)
                return BadRequest();

            var user = await db.Employee
                .Include(e => e.Leaves)
                .SingleAsync(e => e.EmployeeId == id);

            if (leaves != null && user != null && ModelState.IsValid)
            {
                foreach (var leave in leaves)
                {
                    var temp = user.Leaves.SingleOrDefault(li => li.LeaveId == leave.LeaveId);
                    if (temp != null)
                    {
                        temp.StartDate = leave.StartDate;
                        temp.EndDate = leave.EndDate;
                        temp.TypeLeave = leave.TypeLeave;
                        db.Leave.Update(temp);
                    }
                    else return BadRequest();
                }

                if (!LeavesValidator.ValidateLeaves(user.Leaves.ToList()))
                    return BadRequest();
            }
            else return NotFound();

            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// DELETE api/employee/{id}/leaves
        /// Deletes a leaves record.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that is deleted to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpDelete("{id}/leaves")]
        public async Task<ActionResult> DeleteLeaveAsync(int id, [FromBody] List<Leave> leaves)
        {
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var authorizedUser = await db.Employee.SingleOrDefaultAsync(e => e.EmployeeId == thisUser.EmployeeId);
            var maxRole = authorizedUser.MaxRole;
            
            if (authorizedUser.EmployeeId != id && maxRole != RoleType.Manager && maxRole != RoleType.Administrator)
                return BadRequest();

            var user = await db.Employee
                .Include(e => e.Leaves)
                .SingleOrDefaultAsync(e => e.EmployeeId == id);

            if (leaves != null && user != null && ModelState.IsValid)
                foreach (var leave in leaves)
                {
                    var temp = user.Leaves.SingleOrDefault(li => li.LeaveId == leave.LeaveId);
                    if (temp != null)
                        db.Leave.Remove(temp);
                    else return BadRequest();
                }
            else return NotFound();

            db.SaveChanges();
            return Ok();
        }
    }
}
