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

        //TODO Метод GetAllProjects для получения всех проектов
        //TODO Связывание проектов с менеджерами из таблицы ProjectEmployee (уточнить нужно ли)
        //TODO Назначение и снятие менеджера с проекта

        // GET: api/Administrator/GetProject
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

        // PUT: api/Administrator/UpdateProject
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

        // POST: api/Administrator/AddProject
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

        // DELETE: api/Administrator/DeleteProject
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

        // POST: api/Administrator/SetManager
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

        // DELETE: api/Administrator/DeleteManager
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