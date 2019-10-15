using Microsoft.AspNetCore.Mvc;
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
using System.Collections.Generic;
using AutoMapper;
using Task = LTRegistrator.Domain.Entities.Task;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userManager"></param>
        public AdministratorController(DbContext db, IMapper mapper, UserManager<User> userManager) : base(db, mapper)
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
            var managerEmployee = Db.Set<Employee>().Where(e => e.Id == managerId).FirstOrDefault();
            var projectManager = Db.Set<ProjectEmployee>().Where(pe => pe.ProjectId == projectId && pe.Role == RoleType.Manager).FirstOrDefault();
            var projectEmployee = Db.Set<ProjectEmployee>().Where(pe => pe.ProjectId == projectId && pe.EmployeeId == managerId && pe.Role == RoleType.Employee).FirstOrDefault();
            var newProjectManager = new ProjectEmployee { EmployeeId = projectId, ProjectId = projectId, Role = RoleType.Manager };

            if (managerEmployee.MaxRole == RoleType.Manager && projectManager != null && projectEmployee == null)
            {
                Db.Set<ProjectEmployee>().Remove(projectManager);
                Db.Set<ProjectEmployee>().Add(newProjectManager);
                await Db.SaveChangesAsync();
                return Ok();
            }
            else if (managerEmployee.MaxRole == RoleType.Manager && projectEmployee != null)
            {
                Db.Set<ProjectEmployee>().Remove(projectEmployee);
                Db.Set<ProjectEmployee>().Remove(projectManager);
                Db.Set<ProjectEmployee>().Add(newProjectManager);
                await Db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
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
                employee.MaxRole = assignedRole;
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

            if (employee.MaxRole != RoleType.Employee || manager.MaxRole != RoleType.Manager || employee.ManagerId != null) return BadRequest();

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

        /// <summary>
        /// Get employees in project without manager
        /// </summary>
        /// <param name="projectId">Id of project</param>
        /// <response code="200">List employees</response>
        /// <response code="404">Employees not found </response>
        [HttpGet("project/{projectId}/employees")]
        [ProducesResponseType(typeof(List<EmployeeDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetEmployeesByProject([FromRoute] int projectId)
        {
            var employees = await Db.Set<ProjectEmployee>()
                .Include(pe => pe.Project)
                .Include(pe => pe.Employee)
                .Where(pe => pe.ProjectId == projectId && pe.Role == RoleType.Employee)
                .Select(pe => pe.Employee).ToListAsync();
            if (!employees.Any())
            {
                return NotFound();
            }
            return Ok(DtoConverter.ToEmployeeDto(employees));
        }

        /// <summary>
        /// Deletes a project marked as softDeletes
        /// </summary>
        /// <param name="projectId"></param>
        /// <response code="204">The project was successfully deleted.</response>
        /// <response code="404">If project was not found</response>
        /// <response code="409">The found project does not contain the flag SoftDeleted = true</response>
        [HttpDelete("project/{projectId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult> RemoveProject(int projectId)
        {
            var project = await Db.Set<Project>().FirstOrDefaultAsync(p => p.Id == projectId).ConfigureAwait(false);
            if (project == null)
            {
                return NotFound();
            }

            if (!project.SoftDeleted)
            {
                return Conflict();
            }

            Db.Set<Project>().Remove(project);
            await Db.SaveChangesAsync().ConfigureAwait(false);
            return NoContent();
        }

        /// <summary>
        /// Restores property of project SoftDeleted = false
        /// </summary>
        /// <param name="projectId"></param>
        /// <response code="204">The project was successfully restored.</response>
        /// <response code="404">If project was not found</response>
        /// <response code="409">The found project does not contain the flag SoftDeleted = true</response>
        [HttpPut("project/{projectId}/softDeleted")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult> RestoreProject(int projectId)
        {
            var project = await Db.Set<Project>().FirstOrDefaultAsync(p => p.Id == projectId).ConfigureAwait(false);
            if (project == null)
            {
                return NotFound();
            }

            if (!project.SoftDeleted)
            {
                return Conflict();
            }

            project.SoftDeleted = false;
            await Db.SaveChangesAsync().ConfigureAwait(false);

            return NoContent();
        }
    } 
}