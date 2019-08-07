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
        private readonly LTRegistratorDbContext _db;
        private readonly UserManager<User> _userManager;

        public AdministratorController(LTRegistratorDbContext context, UserManager<User> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        /// <summary>
        /// updating project information
        /// PUT: api/Administrator/Project
        /// </summary>
        /// <param name="project">json {ProjectId, Name, projectEmployee}
        /// Name and projectEmployee not obligatory</param>
        /// <returns> "OK" or "not found"</returns>
        [HttpPut("Project/{projectid}")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDto projectdto, [FromRoute] int projectid)
        {
            var temp = _db.Project.SingleOrDefault(p => p.Id == projectid);
            if (temp != null)
            {
                temp.Id = projectid;
                temp.Name = projectdto.Name;
                _db.Project.Update(temp);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// method for assigning a project manager
        /// POST: api/Administrator/setmanager/{managerID}/project/{projectID}
        /// </summary>
        /// <param name="projectid">id of project</param>
        /// <param name="managerid">id of manager</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpPost("setmanager/{managerID}/project/{projectID}")]
        public async Task<IActionResult> SetManager([FromRoute] int projectid, int managerid)
        {
            var managerEmployee = _db.Employee.Where(e => e.Id == managerid).FirstOrDefault();
            var projectManager = _db.ProjectEmployee.Where(pe => pe.ProjectId == projectid && pe.Role == RoleType.Manager).FirstOrDefault();
            var projectEmployee = _db.ProjectEmployee.Where(pe => pe.ProjectId == projectid && pe.EmployeeId == managerid && pe.Role == RoleType.Employee).FirstOrDefault();
            var newProjectManager = new ProjectEmployee { EmployeeId = managerid, ProjectId = projectid, Role = RoleType.Manager };

            if (managerEmployee.MaxRole == RoleType.Manager && projectManager != null && projectEmployee == null)
            {
                _db.ProjectEmployee.Remove(projectManager);
                _db.ProjectEmployee.Add(newProjectManager);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else if (managerEmployee.MaxRole == RoleType.Manager && projectManager == null)
            {
                if (projectEmployee != null)
                {
                    _db.ProjectEmployee.Remove(projectEmployee);
                }
                _db.ProjectEmployee.Add(newProjectManager);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else if (managerEmployee.MaxRole == RoleType.Manager && projectEmployee != null)
            {
                _db.ProjectEmployee.Remove(projectEmployee);
                _db.ProjectEmployee.Remove(projectManager);
                _db.ProjectEmployee.Add(newProjectManager);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// method for removing the manager from the project
        /// DELETE: api/Administrator/DeleteManager/project/{projectId}
        /// </summary>
        /// <param name="projectid"> id of the project whose manager should be deleted</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpDelete("DeleteManager/project/{projectId}")]
        public async Task<IActionResult> DeleteManager([FromRoute] int projectid)
        {
            var currentManager = _db.ProjectEmployee.Where(p => p.ProjectId == projectid && p.Role == RoleType.Manager).FirstOrDefault();
            if (currentManager != null)
            {
                _db.ProjectEmployee.Remove(currentManager);

                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}