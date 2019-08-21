using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System;
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
using Microsoft.AspNetCore.Authorization;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// The controller that provides managers functionality.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly LTRegistratorDbContext _db;
        private readonly UserManager<User> _userManager;
        /// <summary></summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        public ManagerController(LTRegistratorDbContext context, UserManager<User> userManager)
        {
            _db = context;
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
        public ActionResult<List<ProjectDto>> GetManagerProjects(int employeeId)
        {
            var projects = DtoConverter.ToProjectDto(_db.ProjectEmployee
                .Join(_db.Project, p => p.ProjectId, pe => pe.Id, (pe, p) => new { pe, p })
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
            var user = await _db.Employee.FindAsync(employeeId);
            var project = await _db.Project.FirstOrDefaultAsync(p => p.Id == projectId && !p.SoftDeleted);
            var userProject = await _db.ProjectEmployee.SingleOrDefaultAsync(v => v.ProjectId == projectId && v.EmployeeId == employeeId);
            if (user != null && project != null && userProject == null && user.ManagerId != null)
            {
                ProjectEmployee projectEmployee = new ProjectEmployee
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId,
                    Role = RoleType.Employee
                };
                _db.ProjectEmployee.Add(projectEmployee);
                await _db.SaveChangesAsync();
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
            var result = await _db.ProjectEmployee.SingleOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId && !pe.Project.SoftDeleted);
            if (result == null)
            {
                return NotFound();
            }
            _db.ProjectEmployee.Remove(result);
            await _db.SaveChangesAsync();
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
        public ActionResult<List<EmployeeDto>> GetEmployees(int projectId, int employeeId)
        {
            var userProject = _db.ProjectEmployee.SingleOrDefault(v => v.ProjectId == projectId && v.EmployeeId == employeeId && !v.Project.SoftDeleted);
            if (userProject == null)
            {
                return NotFound();
            }
            var employee = DtoConverter.ToEmployeeDto(_db.ProjectEmployee.Join(_db.Employee,
                               e => e.EmployeeId,
                               pe => pe.Id,
                               (pe, e) => new { pe, e }).Where(w => w.pe.ProjectId == projectId && w.pe.Role == RoleType.Employee).Select(user => user.e).OrderByDescending(o => o.ManagerId == employeeId).ToList());
            if (!employee.Any())
                return NotFound();
            return Ok(employee);
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
                projects = _db.Project.ToList();
            }
            else
            {
                projects = await _db.Project.Where(w => !w.SoftDeleted).ToListAsync();
            }

            return projects.Any()
                ? (IActionResult)Ok(DtoConverter.ToProjectDto(projects))
                : NoContent();
        }

        /// <summary>
        /// Adding a new project
        /// </summary>
        /// <param name="projectdto">Data transfer object, required containing Name of project</param>
        /// <response code="200">Created project</response>
        /// <response code="400">Bad request</response>
        /// <response code="403">You do not have sufficient permissions to add a project</response>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpPost("project")]
        [ProducesResponseType(typeof(ProjectDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddProject([FromBody] ProjectDto projectdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var thisUserIdent = HttpContext.User.Identity as ClaimsIdentity;
            if (projectdto != null)
            {
                if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Administrator")))
                {
                    var project = new Project { Name = projectdto.Name };
                    _db.Project.Add(project);
                    await _db.SaveChangesAsync();
                    return Ok(new ProjectDto { Id = project.Id, Name = project.Name });
                }
                else if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Manager")))
                {
                    var project = new Project { Name = projectdto.Name };
                    _db.Project.Add(project);
                    _db.SaveChanges();
                    ProjectEmployee projectEmployee = new ProjectEmployee
                    {
                        ProjectId = project.Id,
                        EmployeeId = thisUser.EmployeeId,
                        Role = RoleType.Manager
                    };
                    _db.ProjectEmployee.Add(projectEmployee);
                    _db.SaveChanges();
                    return Ok(new ProjectDto { Id = project.Id, Name = project.Name });
                }
                else
                {
                    return Forbid("You do not have sufficient permissions to add a project");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// updating project information
        /// </summary>
        /// <param name="projectdto"> Name and projectEmployee not obligatory</param>
        /// <param name="projectid"> Id of the project, information about which will be updated </param>
        /// <response code="200">Information updated</response>
        /// <response code="404">Project not found</response>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpPut("Project/{projectid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDto projectdto, [FromRoute] int projectid)
        {
            var project = _db.Project.SingleOrDefault(p => p.Id == projectid);
            if (project != null)
            {
                project.Name = projectdto.Name;
                _db.Project.Update(project);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
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
            var project = await _db.Project.FindAsync(id);
            var managerEmployee = await _db.ProjectEmployee.SingleOrDefaultAsync(V => V.ProjectId == id && V.Role == RoleType.Manager);

            if (project != null)
            {
                if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Administrator")))
                {
                    var listEmployees = _db.ProjectEmployee.Where(pe => pe.ProjectId == id).ToList();
                    foreach (ProjectEmployee employee in listEmployees)
                    {
                        _db.ProjectEmployee.Remove(employee);
                    }

                    _db.Project.Remove(project);
                    await _db.SaveChangesAsync();

                    return Ok();
                }
                else if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Manager"))
                    && managerEmployee != null && managerEmployee.EmployeeId == thisUser.EmployeeId)
                {
                    project.SoftDeleted = true;
                    await _db.SaveChangesAsync();

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