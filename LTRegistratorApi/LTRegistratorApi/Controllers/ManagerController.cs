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
        //List<Project> to List<ProjectDto>
        private static List<ProjectDto> ToProjectDto(List<Project> projects)
        {
            var result = new List<ProjectDto>();
            foreach (var project in projects)
                result.Add(new ProjectDto { ProjectId = project.ProjectId, Name = project.Name });
            return result;
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
            var projects = ToProjectDto(db.ProjectEmployee.Join(db.Project,
                                     p => p.ProjectId,
                                     pe => pe.ProjectId,
                                     (pe, p) => new { pe, p }).Where(w => w.pe.EmployeeId == EmployeeId && w.pe.RoleType == RoleType.Manager).Select(name => name.p).ToList());
            if (!projects.Any())
                return NotFound();
            return Ok(projects);
        }
        /// <summary>
        /// Post api/manager/project/{ProjectId}/assign/{EmployeeId} 
        /// Add an employee to the project.
        /// </summary>
        /// <param name="ProjectId">ProjectId</param>    
        /// <param name="EmployeeId">EmployeeId</param>    
        /// <returns>Was the operation successful?</returns>
        [HttpPost("project/{ProjectId}/assign/{EmployeeId}")]
        public async Task<ActionResult> AssignEmployeeToProject(int ProjectId, int EmployeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await db.Employee.FindAsync(EmployeeId);
            var project = await db.Project.FindAsync(ProjectId);
            if (user != null && project != null)
            {
                ProjectEmployee projectEmployee = new ProjectEmployee
                {
                    ProjectId = ProjectId,
                    EmployeeId = EmployeeId,
                    RoleType = RoleType.Employee
                };
                db.ProjectEmployee.Add(projectEmployee);
                await db.SaveChangesAsync();
            }
            else NotFound();
            return Ok();
        }
        /// <summary>
        /// DELETE api/manager/project/{projectId}/reassign/{EmployeeId}
        /// Delete employee from project.
        /// </summary>
        /// <param name="ProjectId">ProjectId</param>
        /// <param name="EmployeeId">EmployeeId</param>
        /// <returns>Was the operation successful?</returns>
        [HttpDelete("project/{ProjectId}/reassign/{EmployeeId}")]
        public async Task<ActionResult> ReassignEmployyeFromProject(int ProjectId, int EmployeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
        /// Get api/manager/project/{ProjectId}/employees
        /// Get employees in the project
        /// </summary>
        /// <param name="ProjectId">ProjectId</param>
        /// <returns>Employee list</returns>
        [HttpGet("project/{ProjectId}/employees")]
        public ActionResult<List<EmployeeDto>> GetEmployee(int ProjectId)
        {          
            var employee = ToEmployeeDto(db.ProjectEmployee.Join(db.Employee,
                                     e => e.EmployeeId,
                                     pe => pe.EmployeeId,
                                     (pe, e) => new { pe, e }).Where(w => w.pe.ProjectId == ProjectId && w.pe.RoleType == RoleType.Employee).Select(user => user.e).ToList());
            if (!employee.Any())
                return NotFound();
            return Ok(employee);
        }
        //List<Employee> to List<EmployeeDto>
        private static List<EmployeeDto> ToEmployeeDto(List<Employee> employees)
        {
            var result = new List<EmployeeDto>();
            foreach (var employee in employees)
                result.Add(new EmployeeDto
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    SecondName = employee.SecondName,
                    Mail = employee.Mail,
                    MaxRole = employee.MaxRole                  
                });
            return result;
        }
    }
}

