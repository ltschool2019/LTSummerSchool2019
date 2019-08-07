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

        /// <summary>
        /// Update role claim of user to Manager
        /// </summary>
        /// <param name="employeeid">id of user which should be assigned as manager</param>
        /// <response code="200">Claim updated</response>
        /// <response code="400">User cannot be assigned as manager</response>
        /// <response code="404">Cannot find user</response>
        [HttpPut("AllowBeManager/{employeeid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AllowBeManager([FromRoute] string employeeid)
        {
            var user = await _userManager.FindByIdAsync(employeeid);
            if (user != null)
            {
                var oldclaims = await _userManager.GetClaimsAsync(user);
                var isemployee = oldclaims.Where(c => c.Value == "Employee").FirstOrDefault();
                if (isemployee != null)
                {
                    await _userManager.RemoveClaimsAsync(user, oldclaims);
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, Enum.GetName(typeof(RoleType), 1)));
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

        /// <summary>
        /// Update role claim of user to Employee
        /// </summary>
        /// <param name="employeeid">id of user which should be assigned as employee</param>
        /// <response code="200">Claim updated</response>
        /// <response code="400">User cannot be assigned as employee</response>
        /// <response code="404">Cannot find user</response>
        [HttpPut("DisallowBeManager/{employeeid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DisallowBeManager([FromRoute] string employeeid)
        {
            var user = await _userManager.FindByIdAsync(employeeid);
            if (user != null)
            {
                var oldclaims = await _userManager.GetClaimsAsync(user);
                var isemployee = oldclaims.Where(c => c.Value == "Manager").FirstOrDefault();
                if (isemployee != null)
                {
                    await _userManager.RemoveClaimsAsync(user, oldclaims);
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, Enum.GetName(typeof(RoleType), 0)));
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