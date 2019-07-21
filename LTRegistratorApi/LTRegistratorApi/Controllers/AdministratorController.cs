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

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(Policy = "IsAdministrator")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public AdministratorController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// method for getting the project by id
        /// GET: api/Administrator/GetProject/{id}
        /// </summary>
        /// <param name="id">id of project</param>
        /// <returns>json {ProjectId, Name}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] int id)
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

            return Ok(project);
        }

        /// <summary>
        /// updating project information
        /// PUT: api/Administrator/UpdateProject
        /// </summary>
        /// <param name="project">json {ProjectId, Name, projectEmployee}
        /// Name and projectEmployee not obligatory</param>
        /// <returns> "OK" or "bad request" or "not found"</returns>
        [HttpPut]
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
        [HttpPost]
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
        [HttpDelete("{id}")]
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

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        /// <summary>
        /// method for assigning a project manager
        /// POST: api/Administrator/SetManager
        /// </summary>
        /// <param name="projectemployee">json {ProjectId, EmployeeId, Role}</param>
        /// <returns>"200 ok" or "401 bad request"</returns>
        [HttpPost]
        public async Task<IActionResult> SetManager([FromBody] ProjectEmployee projectemployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProjectEmployee.Add(projectemployee);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// method for removing the manager from the project
        /// DELETE: api/Administrator/DeleteManager
        /// </summary>
        /// <param name="projectemployee">json {ProjectId, EmployeeId, Role}</param>
        /// <returns>"200 ok" or "401 bad request" or "404 not found"</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteManager([FromBody] ProjectEmployee projectemployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pe = _context.ProjectEmployee
                .SingleOrDefault(V => V.ProjectId == projectemployee.ProjectId 
                && V.EmployeeId == projectemployee.EmployeeId 
                && V.Role == projectemployee.Role);

            if (pe != null)
            {
                _context.ProjectEmployee.Remove(pe);
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