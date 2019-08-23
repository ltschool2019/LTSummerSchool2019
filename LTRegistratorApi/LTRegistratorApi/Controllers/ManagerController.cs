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
using Microsoft.AspNetCore.Authorization;

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
        public ManagerController(DbContext db, UserManager<User> userManager) : base(db)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// GET api/manager/{EmployeeId}/projects
        /// Output of all projects of the manager. 
        /// </summary>
        /// <param name="employeeId">EmployeeId</param>
        /// <returns>Manager's projects list</returns>
        [HttpGet("{EmployeeId}/projects")]
        public ActionResult<List<ProjectDto>> GetManagerProjects(int employeeId)
        {
            var projects = DtoConverter.ToProjectDto(Db.Set<ProjectEmployee>().Join(Db.Set<Project>(),
                                     p => p.ProjectId,
                                     pe => pe.Id,
                                     (pe, p) => new { pe, p }).Where(w => w.pe.EmployeeId == employeeId && w.pe.Role == RoleType.Manager).Select(name => name.p).ToList());
            if (!projects.Any())
                return NotFound();
            return Ok(projects);
        }

        /// <summary>
        /// Post api/manager/project/{ProjectId}/assign/{EmployeeId} 
        /// Add a project to the employee.
        /// </summary>
        /// <param name="projectId">ProjectId</param>    
        /// <param name="employeeId">EmployeeId</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPost("project/{ProjectId}/assign/{EmployeeId}")]
        public async Task<ActionResult> AssignProjectToEmployee(int projectId, int employeeId)
        {
            var user = await Db.Set<Employee>().FindAsync(employeeId);
            var project = await Db.Set<Project>().FindAsync(projectId);
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
            else return NotFound();
        }

        /// <summary>
        /// DELETE api/manager/project/{projectId}/reassign/{EmployeeId}
        /// Delete employee from project.
        /// </summary>
        /// <param name="projectId">ProjectId</param>
        /// <param name="employeeId">EmployeeId</param>
        /// <returns>Was the operation successful?</returns>
        [HttpDelete("project/{ProjectId}/reassign/{EmployeeId}")]
        public async Task<ActionResult> ReassignEmployeeFromProject(int projectId, int employeeId)
        {
            var result = await Db.Set<ProjectEmployee>().Where(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId).SingleOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            Db.Set<ProjectEmployee>().Remove(result);
            await Db.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Get api/manager/{EmployeeId}/project/{ProjectId}/employees
        /// Get employees in the project
        /// First the manager people are displayed, then the rest
        /// </summary>
        /// <param name="projectId">ProjectId</param>
        /// <param name="employeeId">EmployeeId of manager</param>
        /// <returns>Employee list</returns>
        [HttpGet("{EmployeeId}/project/{ProjectId}/employees")]
        public ActionResult<List<EmployeeDto>> GetEmployees(int projectId, int employeeId)
        {
            var userProject = Db.Set<ProjectEmployee>().SingleOrDefault(v => v.ProjectId == projectId && v.EmployeeId == employeeId);
            if (userProject == null)
            {
                return NotFound();
            }
            var employee = DtoConverter.ToEmployeeDto(Db.Set<ProjectEmployee>().Join(Db.Set<Employee>(),
                               e => e.EmployeeId,
                               pe => pe.Id,
                               (pe, e) => new { pe, e }).Where(w => w.pe.ProjectId == projectId && w.pe.Role == RoleType.Employee).Select(user => user.e).OrderByDescending(o => o.ManagerId == employeeId).ToList());
            if (!employee.Any())
                return NotFound();
            return Ok(employee);
        }

        /// <summary>
        /// method for getting the the list of all projects
        /// GET: api/Manager/allprojects
        /// </summary>
        /// <returns>list of projects in json {ProjectId, Name}</returns>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpGet("allprojects")]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await Db.Set<Project>().ToListAsync();
            return Ok(DtoConverter.ToProjectDto(projects));
        }

        /// <summary>
        /// adding a new project
        /// POST: api/Manager/project
        /// </summary>
        /// <param name="project">json {Name}</param>
        /// <returns>"201 created" and json {ProjectId, EmployeeId}</returns>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpPost("project")]
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
                    var project = new Project {Name = projectdto.Name };
                    Db.Set<Project>().Add(project);
                    await Db.SaveChangesAsync();
                    return Ok(new ProjectDto { Id = project.Id, Name = project.Name });
                }
                else
                {
                    if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Manager")))
                    {
                        var project = new Project { Name = projectdto.Name };
                        Db.Set<Project>().Add(project);
                        Db.SaveChanges();
                        ProjectEmployee projectEmployee = new ProjectEmployee
                        {
                            ProjectId = project.Id,
                            EmployeeId = thisUser.EmployeeId,
                            Role = RoleType.Manager
                        };
                        Db.Set<ProjectEmployee>().Add(projectEmployee);

                        Db.SaveChanges();
                        return Ok(new ProjectDto { Id = project.Id, Name = project.Name });
                    }
                    else
                    {
                        return Forbid("You do not have sufficient permissions to add a project");
                    }
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// deleting project by id
        /// DELETE: api/Manager/project/{id}
        /// </summary>
        /// <param name="id">id of project to be deleted</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpDelete("project/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var thisUserIdent = HttpContext.User.Identity as ClaimsIdentity;
            var project = await Db.Set<Project>().FindAsync(id);
            var managerEmployee = await Db.Set<ProjectEmployee>().SingleOrDefaultAsync(V => V.ProjectId == id && V.Role == RoleType.Manager);

            if (project != null)
            {
                if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Administrator")) || (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Manager")) && managerEmployee != null && managerEmployee.EmployeeId == thisUser.EmployeeId))
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
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}