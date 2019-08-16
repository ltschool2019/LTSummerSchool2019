using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LTRegistrator.BLL.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "IsAdministrator")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly LTRegistratorDbContext _db;
        private readonly UserManager<User> _userManager;

        public AdministratorController(LTRegistratorDbContext context, UserManager<User> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        /// <summary>
        /// updating project information
        /// PUT: api/Administrator/Project
        /// </summary>
        /// <param name="project">json {ProjectId, Name, projectEmployee}
        /// Name and projectEmployee not obligatory</param>
        /// <returns> "OK" or "not found"</returns>
        [HttpPut("Project/{projectid}")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDto projectdto, [FromRoute] int projectid)
        {
            var temp = _db.Project.SingleOrDefault(p => p.Id == projectid);
            if (temp != null)
            {
                temp.Id = projectid;
                temp.Name = projectdto.Name;
                _db.Project.Update(temp);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// method for assigning a project manager
        /// POST: api/Administrator/setmanager/{managerID}/project/{projectID}
        /// </summary>
        /// <param name="projectid">id of project</param>
        /// <param name="managerid">id of manager</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpPost("setmanager/{managerID}/project/{projectID}")]
        public async Task<IActionResult> SetManager([FromRoute] int projectid, int managerid)
        {
            var managerEmployee = _db.Employee.Where(e => e.Id == managerid).FirstOrDefault();
            var projectManager = _db.ProjectEmployee.Where(pe => pe.ProjectId == projectid && pe.Role == RoleType.Manager).FirstOrDefault();
            var projectEmployee = _db.ProjectEmployee.Where(pe => pe.ProjectId == projectid && pe.EmployeeId == managerid && pe.Role == RoleType.Employee).FirstOrDefault();
            var newProjectManager = new ProjectEmployee { EmployeeId = managerid, ProjectId = projectid, Role = RoleType.Manager };

            if (managerEmployee.MaxRole == RoleType.Manager && projectManager != null && projectEmployee == null)
            {
                _db.ProjectEmployee.Remove(projectManager);
                _db.ProjectEmployee.Add(newProjectManager);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else if (managerEmployee.MaxRole == RoleType.Manager && projectManager == null)
            {
                if (projectEmployee != null)
                {
                    _db.ProjectEmployee.Remove(projectEmployee);
                }
                _db.ProjectEmployee.Add(newProjectManager);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else if (managerEmployee.MaxRole == RoleType.Manager && projectEmployee != null)
            {
                _db.ProjectEmployee.Remove(projectEmployee);
                _db.ProjectEmployee.Remove(projectManager);
                _db.ProjectEmployee.Add(newProjectManager);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// method for removing the manager from the project
        /// DELETE: api/Administrator/DeleteManager/project/{projectId}
        /// </summary>
        /// <param name="projectid"> id of the project whose manager should be deleted</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpDelete("DeleteManager/project/{projectId}")]
        public async Task<IActionResult> DeleteManager([FromRoute] int projectid)
        {
            var currentManager = _db.ProjectEmployee.Where(p => p.ProjectId == projectid && p.Role == RoleType.Manager).FirstOrDefault();
            if (currentManager != null)
            {
                _db.ProjectEmployee.Remove(currentManager);

                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Update role claim of user
        /// </summary>
        /// <param name="employeeId">id of user which should be assigned as employee</param>
        /// <param name="assignedRole">role to be assigned to the employee</param>
        /// <response code="200">Claim updated</response>
        /// <response code="400">User cannot be assigned as employee</response>
        /// <response code="404">Cannot find user</response>
        [HttpPut("SetRole/{employeeId}/{assignedRole}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SetRole([FromRoute] int employeeId, RoleType assignedRole)
        {
            if (assignedRole == RoleType.Administrator) return BadRequest("You cannot designate an employee as an administrator");

            var employee = await _db.Set<Employee>().Include(e => e.User).FirstOrDefaultAsync(e => e.Id == employeeId).ConfigureAwait(false);
            if (employee != null)
            {
                var oldClaims = await _userManager.GetClaimsAsync(employee.User);
                employee.MaxRole = assignedRole;
                if (assignedRole == RoleType.Manager)
                {
                    employee.ManagerId = null;
                }

                await _db.SaveChangesAsync().ConfigureAwait(false);

                await _userManager.RemoveClaimsAsync(employee.User, oldClaims);
                await _userManager.AddClaimAsync(employee.User, new Claim(ClaimTypes.Role, assignedRole.ToString()));
                return Ok();
            }

            return NotFound($"Employee with id = {employeeId} not found");
        }

        /// <summary>
        /// Assigning manager to employee
        /// </summary>
        /// <param name="managerId">Id Manager to be assigned to the employee</param>
        /// <param name="employeeId">Id employee who is assigned manager</param>
        /// <response code="200">Manager assigned to employee</response>
        /// <response code="400">You have selected the manager or the employee does not have a matching role or manager already assigned to selected employee</response>
        /// <response code="404">Manager or employee not found</response>
        [HttpPut("assignmanager/{managerId}/employee/{employeeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> AssignManagerToEmployee([FromRoute] int managerId, int employeeId)
        {
            var employee = await _db.Employee.FindAsync(employeeId);
            var manager = await _db.Employee.FindAsync(managerId);

            if (employee == null || manager == null) return NotFound();

            if (employee.MaxRole != RoleType.Employee || manager.MaxRole != RoleType.Manager || employee.ManagerId != null) return BadRequest();
            
            employee.ManagerId = managerId;
            await _db.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Untying employee from manager
        /// </summary>
        /// <param name="employeeId">Id Employee which must be untied from the manager</param>
        /// <response code="200">Employee untied from manager</response>
        /// <response code="400">Employee does not have a manager</response>
        /// <response code="404">Employee not found </response>
        [HttpPut("untieemployee/{employeeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UntieEmployeeFromManager([FromRoute] int employeeId)
        {
            var employee = await _db.Employee.FindAsync(employeeId);
            if (employee == null) return NotFound();

            if (employee.ManagerId == null) return BadRequest();

            employee.ManagerId = null;
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}