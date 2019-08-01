using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        ApplicationContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        public ManagerController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        /// <summary>
        /// GET api/manager/{EmployeeId}/projects
        /// Output of all projects of the manager. 
        /// </summary>
        /// <param name="EmployeeId">EmployeeId</param>
        /// <returns>Manager's projects list</returns>
        [HttpGet("{EmployeeId}/projects")]
        public ActionResult<List<ProjectDto>> GetManagerProjects(int EmployeeId)
        {
            var projects = DtoConverter.ToProjectDto(db.ProjectEmployee.Join(db.Project,
                                     p => p.ProjectId,
                                     pe => pe.ProjectId,
                                     (pe, p) => new { pe, p }).Where(w => w.pe.EmployeeId == EmployeeId && w.pe.Role == RoleType.Manager).Select(name => name.p).ToList());
            if (!projects.Any())
                return NotFound();
            return Ok(projects);
        }

        /// <summary>
        /// method for getting the the list of all projects
        /// GET: api/Manager/GetAllProjects
        /// </summary>
        /// <returns>list of projects in json {ProjectId, Name}</returns>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpGet("GetProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            using (db)
            {
                await db.Project.LoadAsync();
                var projects = db.Project.Local.ToList();
                return Ok(DtoConverter.ToProjectDto(projects));
            }
        }
        /// <summary>
        /// Post api/manager/project/{ProjectId}/assign/{EmployeeId} 
        /// Add a project to the employee.
        /// </summary>
        /// <param name="ProjectId">ProjectId</param>    
        /// <param name="EmployeeId">EmployeeId</param>    
        /// <returns>Was the operation successful?</returns>
        [HttpPost("project/{ProjectId}/assign/{EmployeeId}")]
        public async Task<ActionResult> AssignProjectToEmployee(int ProjectId, int EmployeeId)
        {
            var user = await db.Employee.FindAsync(EmployeeId);
            var project = await db.Project.FindAsync(ProjectId);
            var userproject = await db.ProjectEmployee.SingleOrDefaultAsync(V => V.ProjectId == ProjectId && V.EmployeeId == EmployeeId);
            if (user != null && project != null && userproject == null && user.ManagerId != null)
            {
                ProjectEmployee projectEmployee = new ProjectEmployee
                {
                    ProjectId = ProjectId,
                    EmployeeId = EmployeeId,
                    Role = RoleType.Employee
                };
                db.ProjectEmployee.Add(projectEmployee);
                await db.SaveChangesAsync();
                return Ok();
            }
            else return NotFound();
        }
        /// <summary>
        /// DELETE api/manager/project/{projectId}/reassign/{EmployeeId}
        /// Delete employee from project.
        /// </summary>
        /// <param name="ProjectId">ProjectId</param>
        /// <param name="EmployeeId">EmployeeId</param>
        /// <returns>Was the operation successful?</returns>
        [HttpDelete("project/{ProjectId}/reassign/{EmployeeId}")]
        public async Task<ActionResult> ReassignEmployeeFromProject(int ProjectId, int EmployeeId)
        {
            var result = await db.ProjectEmployee.Where(pe => pe.ProjectId == ProjectId && pe.EmployeeId == EmployeeId).SingleOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            db.ProjectEmployee.Remove(result);
            await db.SaveChangesAsync();
            return Ok();
        }
        /// <summary>
        /// Get api/manager/{EmployeeId}/project/{ProjectId}/employees
        /// Get employees in the project
        /// First the manager people are displayed, then the rest
        /// </summary>
        /// <param name="ProjectId">ProjectId</param>
        /// <param name="EmployeeId">EmployeeId of manager</param>
        /// <returns>Employee list</returns>
        [HttpGet("{EmployeeId}/project/{ProjectId}/employees")]
        public ActionResult<List<EmployeeDto>> GetEmployees(int ProjectId, int EmployeeId)
        {
            var userproject = db.ProjectEmployee.SingleOrDefault(V => V.ProjectId == ProjectId && V.EmployeeId == EmployeeId);
            if (userproject == null)
            {
                return NotFound();
            }
            var employee = DtoConverter.ToEmployeeDto(db.ProjectEmployee.Join(db.Employee,
                               e => e.EmployeeId,
                               pe => pe.EmployeeId,
                               (pe, e) => new { pe, e }).Where(w => w.pe.ProjectId == ProjectId && w.pe.Role == RoleType.Employee).Select(user => user.e).OrderByDescending(o => o.ManagerId == EmployeeId).ToList());
            if (!employee.Any())
                return NotFound();
            return Ok(employee);
        }

        /// <summary>
        /// adding a new project
        /// POST: api/Manager/AddProject
        /// </summary>
        /// <param name="project">json {Name}</param>
        /// <returns>"201 created" and json {ProjectId, EmployeeId}</returns>
        [Authorize(Policy = "IsManagerOrAdministrator")]
        [HttpPost("AddProject")]
        public async Task<IActionResult> AddProject([FromBody] ProjectDto projectdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var thisUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var thisUserIdent = HttpContext.User.Identity as ClaimsIdentity;
            if (thisUser != null && projectdto != null)
            {
                if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Administrator")))
                {
                    var project = new Project { ProjectId = projectdto.ProjectId, Name = projectdto.Name };
                    db.Project.Add(project);
                    await db.SaveChangesAsync();
                    return Ok(new ProjectDto { ProjectId = project.ProjectId, Name = project.Name });
                }
                else
                {
                    if (thisUserIdent.HasClaim(c =>
                            (c.Type == ClaimTypes.Role && c.Value == "Manager")))
                    {
                        var project = new Project { ProjectId = projectdto.ProjectId, Name = projectdto.Name };
                        db.Project.Add(project);
                        ProjectEmployee projectEmployee = new ProjectEmployee
                        {
                            ProjectId = project.ProjectId,
                            EmployeeId = thisUser.EmployeeId,
                            Role = RoleType.Manager
                        };
                        db.ProjectEmployee.Add(projectEmployee);

                        db.SaveChanges();
                        return Ok(new ProjectDto { ProjectId = project.ProjectId, Name = project.Name });
                    }
                    else
                    {
                        return BadRequest();
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
        /// DELETE: api/Manager/DeleteProject/{id}
        /// </summary>
        /// <param name="id">id of project to be deleted</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpDelete("deleteProject/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            var thisUser = await _userManager.GetUserAsync(HttpContext.User);

            var project = await db.Project.FindAsync(id);
            
            var managerEmployee = db.ProjectEmployee
                .Where(pd => pd.ProjectId == id && pd.EmployeeId == thisUser.EmployeeId && pd.Role == RoleType.Manager)
                .FirstOrDefault();

            if (project == null && managerEmployee == null)
            {
                return NotFound();
            }
            else
            {
                var listEmployees = db.ProjectEmployee.Where(pe => pe.ProjectId == id).ToList();
                foreach (ProjectEmployee employee in listEmployees)
                {
                    db.ProjectEmployee.Remove(employee);
                }

                db.Project.Remove(project);
                await db.SaveChangesAsync();

                return Ok(project);
            }
        }
    }
}