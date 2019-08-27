using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTRegistrator.BLL.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// The controller that provides managers functionality.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : BaseController
    {
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userManager"></param>
        public ManagerController(DbContext db, UserManager<User> userManager) : base(db)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Output of all projects of the manager. 
        /// </summary>
        /// <param name="employeeId">EmployeeId</param>
        /// <returns>Manager's projects list</returns>
        /// <response code="200">Manager's projects list</response>
        /// <response code="404">Manager not found or manager have no projects</response>
        [HttpGet("{EmployeeId}/projects")]
        [ProducesResponseType(typeof(List<ProjectDto>), 200)]
        [ProducesResponseType(404)]
        public ActionResult GetManagerProjects(int employeeId)
        {
            var projects = DtoConverter.ToProjectDto(Db.Set<ProjectEmployee>()
                .Join(Db.Set<Project>(), p => p.ProjectId, pe => pe.Id, (pe, p) => new { pe, p })
                .Where(w => w.pe.EmployeeId == employeeId && w.pe.Role == RoleType.Manager && !w.p.SoftDeleted)
                .Select(name => name.p)
                .ToList());

            if (!projects.Any())
                return NotFound();
            return Ok(projects);
        }

        /// <summary>
        /// Add a project to the employee.
        /// </summary>
        /// <param name="projectId">ProjectId</param>    
        /// <param name="employeeId">EmployeeId</param>
        /// <returns>Was the operation successful?</returns>
        /// <response code="200">Project assigned to employee</response>
        /// <response code="404">Employee or project not found</response>
        [HttpPost("project/{ProjectId}/assign/{EmployeeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> AssignProjectToEmployee(int projectId, int employeeId)
        {
            var user = await Db.Set<Employee>().FindAsync(employeeId);
            var project = await Db.Set<Project>().FirstOrDefaultAsync(p => p.Id == projectId && !p.SoftDeleted);
            var userproject = await Db.Set<ProjectEmployee>().SingleOrDefaultAsync(V => V.ProjectId == projectId && V.EmployeeId == employeeId);
            if (user != null && project != null && userproject == null && user.ManagerId != null)
            {
                ProjectEmployee projectEmployee = new ProjectEmployee
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId,
                    Role = RoleType.Employee
                };
                Db.Set<ProjectEmployee>().Add(projectEmployee);
                await Db.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// Delete employee from project.
        /// </summary>
        /// <param name="projectId">ProjectId</param>
        /// <param name="employeeId">EmployeeId</param>
        /// <returns>Was the operation successful?</returns>
        /// <response code="200">Project reassigned to employee</response>
        /// <response code="404">Employee or project not found</response>
        [HttpDelete("project/{ProjectId}/reassign/{EmployeeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> ReassignEmployeeFromProject(int projectId, int employeeId)
        {
            var result = await Db.Set<ProjectEmployee>().SingleOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId && !pe.Project.SoftDeleted);
            if (result == null)
            {
                return NotFound();
            }
            Db.Set<ProjectEmployee>().Remove(result);
            await Db.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Get employees in the project
        /// First the manager people are displayed, then the rest
        /// </summary>
        /// <param name="projectId">ProjectId</param>
        /// <param name="employeeId">EmployeeId of manager</param>
        /// <returns>Employee list</returns>
        /// <response code="200">Employees list</response>
        /// <response code="404">Employees or project not found</response>
        [HttpGet("{EmployeeId}/project/{ProjectId}/employees")]
        [ProducesResponseType(typeof(List<EmployeeDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetEmployees(int projectId, int employeeId)
        {
            var manager = await Db.Set<Employee>().FirstOrDefaultAsync(e => e.Id == employeeId && e.ManagerId == null && e.MaxRole == RoleType.Manager).ConfigureAwait(false);
            if (manager == null)
            {
                return NotFound(new { Message = $"Employee with id = {employeeId} not found or employee is not a manager" });
            }

            var project = await Db.Set<Project>().FirstOrDefaultAsync(p => p.Id == projectId).ConfigureAwait(false);
            if (project == null)
            {
                return NotFound(new { Message = $"Project with id = {projectId} not found" });
            }

            var employees = await Db.Set<ProjectEmployee>()
                .Where(pe => pe.Employee.ManagerId == employeeId && pe.ProjectId == projectId)
                .Select(pe => pe.Employee)
                .Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Project)
                .ToListAsync();

            return Ok(DtoConverter.ToEmployeeDto(employees));
        }

        /// <summary>
        /// Method for getting the the list of all projects
        /// </summary>
        /// <returns>list of projects</returns>
        /// <response code="200">Projects list</response>
        /// <response code="204">There's no projects</response>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpGet("allprojects")]
        [ProducesResponseType(typeof(List<ProjectDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetProjects()
        {
            var thisUserIdent = HttpContext.User.Identity as ClaimsIdentity;
            if (thisUserIdent == null) return BadRequest();

            if (!thisUserIdent.HasClaim(c =>
                c.Type == ClaimTypes.Role && (c.Value == "Administrator" || c.Value == "Manager"))) return BadRequest();

            List<Project> projects;
            if (thisUserIdent.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Administrator"))
            {
                projects = Db.Set<Project>().ToList();
            }
            else
            {
                projects = await Db.Set<Project>().Where(w => !w.SoftDeleted).ToListAsync();
            }

            return projects.Any()
                ? (IActionResult)Ok(DtoConverter.ToProjectDto(projects))
                : NoContent();
        }

        /// <summary>
        /// Adding a new project
        /// </summary>
        /// <param name="projectDto">Data transfer object, required containing Name of project</param>
        /// <response code="200">Created project</response>
        /// <response code="400">Bad request</response>
        /// <response code="403">You do not have sufficient permissions to add a project</response>
        /// <response code="409">Project with this name already exist</response>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpPost("project")]
        [ProducesResponseType(typeof(ProjectDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> AddProject([FromBody] ProjectDto projectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await Db.Set<Project>().AnyAsync(p => p.Name == projectDto.Name).ConfigureAwait(false))
            {
                return Conflict(new { Message = $"Project with name {projectDto.Name} already exist" });
            }

            var project = new Project
            {
                Name = projectDto.Name
            };
            var userClaims = (ClaimsIdentity)User.Identity;
            var isManager = userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Manager");
            if (isManager)
            {
                var employeeIdFromClaims = User.FindFirstValue("EmployeeID");
                if (!int.TryParse(employeeIdFromClaims, out var employeeId))
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
                var manager = await Db.Set<Employee>()
                    .FirstOrDefaultAsync(e => e.Id == Convert.ToInt32(employeeId)).ConfigureAwait(false);
                if (manager == null) return NotFound(new { Message = $"Employee with id = {employeeId}" });

                project.ProjectEmployees = new List<ProjectEmployee>
                {
                    new ProjectEmployee
                    {
                        EmployeeId = employeeId,
                        Role = RoleType.Manager
                    }
                };
            }

            Db.Set<Project>().Add(project);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            return Ok(new ProjectDto { Id = project.Id, Name = project.Name });
        }

        /// <summary>
        /// updating project information
        /// </summary>
        /// <param name="projectDto"> Name and projectEmployee not obligatory</param>
        /// <param name="projectId"> Id of the project, information about which will be updated </param>
        /// <response code="200">Information updated</response>
        /// <response code="404">Project not found</response>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpPut("Project/{projectId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDto projectDto, [FromRoute] int projectId)
        {
            var project = Db.Set<Project>().SingleOrDefault(p => p.Id == projectId);
            if (project == null) return NotFound();

            project.Name = projectDto.Name;
            await Db.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Deleting project by id
        /// </summary>
        /// <param name="id">id of project to be deleted</param>
        /// <response code="200">Created project</response>
        /// <response code="400">Bad request</response>
        /// <response code="403">You do not have sufficient permissions to delete a project</response>
        /// <response code="404">Project not found</response>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpDelete("project/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var thisUserIdent = HttpContext.User.Identity as ClaimsIdentity;
            var project = await Db.Set<Project>().FindAsync(id);
            var managerEmployee = await Db.Set<ProjectEmployee>().SingleOrDefaultAsync(V => V.ProjectId == id && V.Role == RoleType.Manager);

            if (project != null)
            {
                if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Administrator")))
                {
                    var listEmployees = Db.Set<ProjectEmployee>().Where(pe => pe.ProjectId == id).ToList();
                    foreach (ProjectEmployee employee in listEmployees)
                    {
                        Db.Set<ProjectEmployee>().Remove(employee);
                    }

                    Db.Set<Project>().Remove(project);
                    await Db.SaveChangesAsync();

                    return Ok();
                }
                else if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Manager"))
                    && managerEmployee != null && managerEmployee.EmployeeId == thisUser.EmployeeId)
                {
                    project.SoftDeleted = true;
                    await Db.SaveChangesAsync();

                    return Ok();
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}