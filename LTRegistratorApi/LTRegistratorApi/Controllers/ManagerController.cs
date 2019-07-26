using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
        public ManagerController(ApplicationContext context)
        {
            db = context;
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
    }
}