using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using LTRegistrator.BLL.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class TaskController : ControllerBase
    {
        private readonly LTRegistratorDbContext _db;
        private readonly UserManager<User> _userManager;

        public TaskController(LTRegistratorDbContext context, UserManager<User> userManager)
        {
            _db = context;
            _userManager = userManager;
        }
        /// <summary>
        /// POST api/task/project/{projectId}
        /// Adding project tasks
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <param name="task">json {Name, List<{Day, Hours}></param>
        /// <returns>"200 ok" or "400 Bad Request" or "401 Unauthorized"</returns>
        [HttpPost("project/{projectId}")]
        public async Task<ActionResult> AddTask([FromRoute] int projectId, [FromBody] TaskInputDto task)
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var thisUser = 
                await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var authorizedUser =
                await _db.Set<Employee>().SingleOrDefaultAsync(
                    e => e.Id == thisUser.EmployeeId);
            var templateTypeProject = _db.Project.Where(p => p.TemplateType == TemplateType.HoursPerProject && p.Id == projectId).FirstOrDefault();
            var employeeProject = _db.ProjectEmployee.Where(pe => pe.ProjectId == projectId && pe.EmployeeId == thisUser.EmployeeId).FirstOrDefault();
            var nameTask = _db.Task.Where(t => (t.Name == task.Name || t.Name == templateTypeProject.Name)  && t.ProjectId == projectId && t.EmployeeId == thisUser.EmployeeId).FirstOrDefault(); 
            if (nameTask == null && templateTypeProject != null && task != null && templateTypeProject.Name == task.Name && employeeProject != null)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        LTRegistrator.Domain.Entities.Task newTask = new LTRegistrator.Domain.Entities.Task
                        {
                            EmployeeId = authorizedUser.Id,
                            ProjectId = projectId,
                            Name = task.Name
                        };
                        _db.Task.Add(newTask);
                        
                        foreach (var item in task.TaskNotes)
                        {                          
                                TaskNote taskNote = new TaskNote
                                {
                                    TaskId = newTask.Id,
                                    Day = item.Day,
                                    Hours = item.Hours
                                };
                                _db.TaskNote.Add(taskNote);                          
                        }
                        await _db.SaveChangesAsync();
                        transaction.Commit();
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return BadRequest();      
        }
    }
}