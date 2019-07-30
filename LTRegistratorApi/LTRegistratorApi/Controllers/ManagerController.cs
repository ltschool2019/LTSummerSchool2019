using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTRegistrator.DAL;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
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
        private readonly LTRegistratorDbContext _db;
        public ManagerController(LTRegistratorDbContext context)
        {
            _db = context;
        }
        /// <summary>
        /// GET api/manager/{EmployeeId}/projects
        /// Output of all projects of the manager. 
        /// </summary>
        /// <param name="employeeId">EmployeeId</param>
        /// <returns>Manager's projects list</returns>
        [HttpGet("{EmployeeId}/projects")]
        public ActionResult<List<ProjectDto>> GetManagerProjects(Guid employeeId)
        {
            var projects = DtoConverter.ToProjectDto(_db.ProjectEmployee.Join(_db.Project,
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
        /// <param name="ProjectId">ProjectId</param>    
        /// <param name="EmployeeId">EmployeeId</param>    
        /// <returns>Was the operation successful?</returns>
        [HttpPost("project/{ProjectId}/assign/{EmployeeId}")]
        public async Task<ActionResult> AssignProjectToEmployee(Guid projectId, Guid employeeId)
        {
            var user = await _db.Employee.FindAsync(employeeId);
            var project = await _db.Project.FindAsync(projectId);
            var userproject = await _db.ProjectEmployee.SingleOrDefaultAsync(V => V.ProjectId == projectId && V.EmployeeId == employeeId);
            if (user != null && project != null && userproject == null && user.ManagerId != null)
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
        public async Task<ActionResult> ReassignEmployeeFromProject(Guid projectId, Guid employeeId)
        {
            var result = await _db.ProjectEmployee.Where(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId).SingleOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            _db.ProjectEmployee.Remove(result);
            await _db.SaveChangesAsync();
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
        public ActionResult<List<EmployeeDto>> GetEmployees(Guid projectId, Guid employeeId)
        {
            var userProject = _db.ProjectEmployee.SingleOrDefault(v => v.ProjectId == projectId && v.EmployeeId == employeeId);
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
    }
}