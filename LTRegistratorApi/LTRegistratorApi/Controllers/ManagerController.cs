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
    /// Methods of managers. 
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
        public async Task<ActionResult<List<Project>>> GetManagerProjects(int EmployeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await db.ProjectEmployee.Join(db.Project,
                                     p => p.ProjectId,
                                     pe => pe.ProjectId,
                                     (pe, p) => new { pe, p }).Where(w => w.pe.EmployeeId == EmployeeId && w.pe.Role == "Manager").Select(name => new { name.p.Name }).ToListAsync();
            if (!result.Any())
                return NotFound();
            return Ok(result);
        }
        /// <summary>
        /// Post api/manager/{ProjectId}/{EmployeeId}/add
        /// Add an employee to the project.
        /// </summary>
        /// <param name="ProjectId">ProjectId</param>    
        /// <param name="EmployeeId">EmployeeId</param>    
        /// <returns>Was the operation successful?</returns>
        [HttpPost("{ProjectId}/{EmployeeId}/add")]
        public async Task<ActionResult> PostEmployeeToProject(int ProjectId, int EmployeeId)
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
                    Role = "Employee"
                };
                db.ProjectEmployee.Add(projectEmployee);
                await db.SaveChangesAsync();
            }
            else NotFound();
            return Ok();
        }
        /// <summary>
        /// DELETE api/manager/{ProjectId}/{EmployeeId}/delete
        /// Delete employee from project.
        /// </summary>
        /// <param name="ProjectId">ProjectId</param>
        /// <param name="EmployeeId">EmployeeId</param>
        /// <returns>Was the operation successful?</returns>
        [HttpDelete("{ProjectId}/{EmployeeId}/delete")]
        public async Task<ActionResult> DeleteEmployeeFromProject(int ProjectId, int EmployeeId)
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
        /// Get api/manager/{ProjectId}/employee
        /// Get employees in the project
        /// </summary>
        /// <param name="ProjectId">ProjectId</param>
        /// <returns>Employee list</returns>
        [HttpGet("{ProjectId}/employee")]
        public async Task<ActionResult<List<Project>>> GetEmployee(int ProjectId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await db.ProjectEmployee.Join(db.Employee,
                                     e => e.EmployeeId,
                                     pe => pe.EmployeeId,
                                     (pe, e) => new { pe, e }).Where(w => w.pe.ProjectId == ProjectId && w.pe.Role == "Employee").Select(user => new {user.e.FirstName, user.e.SecondName}).ToListAsync();
            if (!result.Any())
                return NotFound();
            return Ok(result);
        }
    }
}

