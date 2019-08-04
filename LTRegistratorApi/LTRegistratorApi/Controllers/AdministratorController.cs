using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using LTRegistrator.BLL.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "IsAdministrator")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly LTRegistratorDbContext _db;

        public AdministratorController(LTRegistratorDbContext context)
        {
            _db = context;
        }
        /// <summary>
        /// updating project information
        /// PUT: api/Administrator/UpdateProject
        /// </summary>
        /// <param name="project">json {ProjectId, Name, projectEmployee}
        /// Name and projectEmployee not obligatory</param>
        /// <returns> "OK" or "bad request" or "not found"</returns>
        [HttpPut("UpdateProject")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDto projectdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var temp = _db.Project.SingleOrDefault(p => p.Id == projectdto.Id);
            if (temp != null)
            {
                temp.Id = projectdto.Id;
                temp.Name = projectdto.Name;
                _db.Project.Update(temp);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }

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

            var project = await _db.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            else
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

        }

        /// <summary>
        /// method for assigning a project manager
        /// POST: api/Administrator/setmanager/project/{projectId}/manager/{managerId}
        /// </summary>
        /// <param name="projectid">id of project</param>
        /// <param name="managerid">id of manager</param>
        /// <returns>"200 ok" or "404 not found"</returns>
        [HttpPost("setmanager/project/{projectId}/manager/{managerId}")]
        public async Task<IActionResult> SetManager([FromRoute] int projectid, int managerid)
        {
            var managerEmployee = _db.Employee.Where(e => e.Id == managerid).FirstOrDefault();
            var projectemployee = _db.ProjectEmployee.Where(pe => pe.ProjectId == projectid).FirstOrDefault();
            if (managerEmployee.MaxRole == RoleType.Manager && projectemployee != null)
            {
                projectemployee.EmployeeId = managerid;
                projectemployee.Role = RoleType.Manager;
                await _db.SaveChangesAsync();
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// method for removing the manager from the project
        /// DELETE: api/Administrator/DeleteManager
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
                return BadRequest();
            }
        }
        
        private bool ProjectExists(int id)
        {
            return _db.Project.Any(e => e.Id == id);
        }
    }
}