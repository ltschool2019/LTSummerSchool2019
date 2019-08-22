﻿using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System.Linq;
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
    /// <summary>
    /// Controller providing basic administrator operations
    /// </summary>
    [Route("api/[controller]")]
    [Authorize(Policy = "IsAdministrator")]
    [ApiController]
    public class AdministratorController : BaseController
    {
        private readonly UserManager<User> _userManager;

        public AdministratorController(DbContext context, UserManager<User> userManager) : base(context)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// updating project information
        /// PUT: api/Administrator/Project
        /// </summary>
        /// <param name="projectdto"> Name and projectEmployee not obligatory</param>
        /// <param name="projectid"> Id of the project, information about which will be updated </param>
        /// <response code="200">Information updated</response>
        /// <response code="404">Project not found</response>
        [HttpPut("Project/{projectid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDto projectdto, [FromRoute] int projectid)
        {
            var project = Db.Set<Project>().SingleOrDefault(p => p.Id == projectid);
            if (project != null)
            {
                project.Name = projectdto.Name;
                Db.Set<Project>().Update(project);
                await Db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Method for assigning manager to project
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <param name="managerId">id of manager</param>
        /// <response code="200"> Manager assigned to project</response>
        /// <response code="400"> Incorrect input</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost("setmanager/{managerID}/project/{projectID}")]
        public async Task<IActionResult> SetManager([FromRoute] int projectId, int managerId)
        {
            var manager = await Db.Set<Employee>().Include(e => e.User).FirstOrDefaultAsync(e => e.Id == managerId).ConfigureAwait(false);
            if (manager == null)
            {
                return NotFound(new { Message = $"Employee with Id = {managerId} not found" });
            }

            if (manager.ManagerId != null)
            {
                return BadRequest(new { Message = $"Employee with Id = {managerId} belongs to the manager with id = {manager.ManagerId}" });
            }

            var project = await Db.Set<Project>().FirstOrDefaultAsync(p => p.Id == projectId).ConfigureAwait(false);
            if (project == null)
            {
                return NotFound(new { Message = $"Project with id = {projectId} not found" });
            }

            if (await Db.Set<ProjectEmployee>().AnyAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == managerId).ConfigureAwait(false))
            {
                return BadRequest(new { Message = $"Project with id = {projectId} already contains manager with id = {managerId}" });
            }

            var employeeClaims = await _userManager.GetClaimsAsync(manager.User).ConfigureAwait(false);
            if (!employeeClaims.Any(c => c.Type == ClaimTypes.Role && c.Value == RoleType.Manager.ToString()))
            {
                return BadRequest(new { Message = "You cannot appoint an employee as a project manager" });
            }

            Db.Set<ProjectEmployee>().Add(new ProjectEmployee
            {
                ProjectId = projectId,
                EmployeeId = managerId
            });
            await Db.SaveChangesAsync().ConfigureAwait(false);

            return Ok();
        }

        /// <summary>
        /// method for removing the manager from the project
        /// </summary>
        /// <param name="projectId"> id of the project whose manager should be deleted</param>
        /// <response code="200"> Manager removed </response>
        /// <response code="404"> Project or manager not found </response>
        [HttpDelete("DeleteManager/project/{projectId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteManager([FromRoute] int projectId)
        {
            var project = await Db.Set<Project>().Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employee).FirstOrDefaultAsync(p => p.Id == projectId).ConfigureAwait(false);
            if (project == null)
            {
                return NotFound(new { Message = $"Project with id = {projectId} not found" });
            }

            var managerProject = project.ProjectEmployees.FirstOrDefault(p => p.Employee.ManagerId == null);
            if (managerProject == null)
            {
                return BadRequest(new { Message = "The project does not contain a manager" });
            }
            Db.Set<ProjectEmployee>().Remove(managerProject);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            return Ok();
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

            var employee = await Db.Set<Employee>().Include(e => e.User).FirstOrDefaultAsync(e => e.Id == employeeId).ConfigureAwait(false);
            if (employee != null)
            {
                var oldClaims = await _userManager.GetClaimsAsync(employee.User).ConfigureAwait(false);
                if (assignedRole == RoleType.Manager)
                {
                    employee.ManagerId = null;
                }
                await Db.SaveChangesAsync().ConfigureAwait(false);
                

                await _userManager.RemoveClaimsAsync(employee.User, oldClaims).ConfigureAwait(false);
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
            var employee = await Db.Set<Employee>().FindAsync(employeeId);
            var manager = await Db.Set<Employee>().FindAsync(managerId);

            if (employee == null || manager == null) return NotFound();

            if (employee.ManagerId == null || manager.ManagerId != null) return BadRequest();

            employee.ManagerId = managerId;
            await Db.SaveChangesAsync();
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
            var employee = await Db.Set<Employee>().FindAsync(employeeId);
            if (employee == null) return NotFound();

            if (employee.ManagerId == null) return BadRequest();

            employee.ManagerId = null;
            await Db.SaveChangesAsync();
            return Ok();
        }
    }
}