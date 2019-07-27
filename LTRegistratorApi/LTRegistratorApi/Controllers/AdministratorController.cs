using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTRegistratorApi.Model;
using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "IsAdministrator")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministratorController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// method for getting the project by id
        /// GET: api/Administrator/GetProjects
        /// </summary>
        /// <returns>list of projects in json {ProjectId, Name}</returns>
        [HttpGet("GetProjects")]
        public async Task<IActionResult> GetProjects()
        {
            using(_context)
            {
                _context.Project.Load();
                var projects = _context.Project.Local.ToList();
                return Ok(projects);
            }
        }

        /// <summary>
        /// updating project information
        /// PUT: api/Administrator/UpdateProject
        /// </summary>
        /// <param name="project">json {ProjectId, Name, projectEmployee}
        /// Name and projectEmployee not obligatory</param>
        /// <returns> "OK" or "bad request" or "not found"</returns>
        [HttpPut("UpdateProject")]
        public async Task<IActionResult> UpdateProject([FromBody] Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(project.ProjectId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        /// <summary>
        /// adding a new project
        /// POST: api/Administrator/AddProject
        /// </summary>
        /// <param name="project">json {Name}</param>
        /// <returns>"201 created" and json {ProjectId, "Name", "projectEmployee"}</returns>
        [HttpPost("AddProject")]
        public async Task<IActionResult> AddProject([FromBody] Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Project.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.ProjectId }, project);
        }

        /// <summary>
        /// deleting project by id
        /// DELETE: api/Administrator/DeleteProject/{id}
        /// </summary>
        /// <param name="id">id of project</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpDelete("DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                var listEmployees = _context.ProjectEmployee.Where(pe => pe.ProjectId == id).ToList();
                foreach (ProjectEmployee employee in listEmployees)
                {
                    _context.ProjectEmployee.Remove(employee);
                }

                _context.Project.Remove(project);
                await _context.SaveChangesAsync();

                return Ok();
            }

        }

        /// <summary>
        /// method for assigning a project manager
        /// POST: api/Administrator/SetManager
        /// </summary>
        /// <param name="projectemployee">json {ProjectId, EmployeeId, Role}</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpPost("setmanager/project/{projectId}/manager/{managerId}")]
        public async Task<IActionResult> SetManager([FromRoute] int projectid, int managerid)
        {
            var currentManager = _context.ProjectEmployee.Where(p => p.ProjectId == projectid && p.Role == RoleType.Manager).FirstOrDefault();
            var oldemployee = _context.ProjectEmployee.Where(p => p.ProjectId == projectid && p.EmployeeId == managerid && p.Role == RoleType.Employee).FirstOrDefault();

            if (currentManager != null)
            {
                _context.ProjectEmployee.Remove(currentManager);
                if (oldemployee != null)
                {
                    _context.ProjectEmployee.Remove(oldemployee);
                }

                ProjectEmployee projectManager = new ProjectEmployee
                {
                    ProjectId = projectid,
                    EmployeeId = managerid,
                    Role = RoleType.Manager
                };
                _context.ProjectEmployee.Add(projectManager);

                var managerEmployee = _context.Employee.Where(e => e.EmployeeId == managerid).FirstOrDefault();
                if (managerEmployee.MaxRole == RoleType.Employee)
                {
                    managerEmployee.MaxRole = RoleType.Manager;
                }
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// method for removing the manager from the project
        /// DELETE: api/Administrator/DeleteManager
        /// </summary>
        /// <param name="projectemployee">json {ProjectId, EmployeeId, Role}</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpDelete("DeleteManager/project/{projectId}")]
        public async Task<IActionResult> DeleteManager([FromRoute] int projectid)
        {
            var currentManager = _context.ProjectEmployee.Where(p => p.ProjectId == projectid && p.Role == RoleType.Manager).FirstOrDefault();
            if (currentManager != null)
            {
                var otherProjectsManager = _context.ProjectEmployee.Where(p => p.Role == RoleType.Manager && p.EmployeeId == currentManager.EmployeeId).ToList();
                if (otherProjectsManager.Count() == 1)
                {
                    var managerEmployee = _context.Employee.Where(e => e.EmployeeId == currentManager.EmployeeId).FirstOrDefault();
                    managerEmployee.MaxRole = RoleType.Employee;
                }

                _context.ProjectEmployee.Remove(currentManager);

                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        
        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }
    }
}