using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Model;
using LTRegistratorApi.Model.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : BaseController
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService, DbContext db, IMapper mapper) : base(db, mapper)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Get project for current user
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns list of projects</response>
        [HttpGet]
        [ProducesResponseType(typeof(ProjectDto[]), 200)]
        public async Task<ActionResult> GetProjects()
        {
            var projects = await _projectService.GetProjects(CurrentEmployeeId);

            return Ok(Mapper.Map<ProjectDto[]>(projects));
        }

        /// <summary>
        /// Get project by id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        /// <response code="200">Returns the found project</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        [HttpGet("{projectId}")]
        [ProducesResponseType(typeof(ProjectFullDto), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetProject([FromRoute] int projectId)
        {
            var project = await _projectService.GetByIdAsync(CurrentEmployeeId, projectId);

            return Ok(Mapper.Map<ProjectFullDto>(project));
        }

        /// <summary>
        /// Adding a new project
        /// </summary>
        /// <param name="projectFullDto">Data transfer object, required containing Name of project</param>
        /// <response code="200">Created project</response>
        /// <response code="400">Bad request</response>
        /// <response code="403">You do not have sufficient permissions to add a project</response>
        /// <response code="409">Project with this name already exist</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> AddProject([FromBody] ProjectFullDto projectFullDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _projectService.AddAsync(CurrentEmployeeId, Mapper.Map<Project>(projectFullDto));

            return Ok(new ProjectDto { Id = result.Id, Name = result.Name });
        }

        /// <summary>
        /// Update project
        /// </summary>
        /// <param name="projectFullDto"></param>
        /// <returns></returns>
        /// <response code="200">Project was updated</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project was not found</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProject(ProjectFullDto projectFullDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _projectService.UpdateAsync(CurrentEmployeeId, Mapper.Map<Project>(projectFullDto));

            return Ok();
        }

        /// <summary>
        /// Delete project by id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        /// <response code="200">Project successfully deleted</response>
        /// <response code="403">Access denied</response>
        /// <response code="403">Project not found</response>
        [HttpDelete("{projectId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteProject(int projectId)
        {
            await _projectService.RemoveAsync(CurrentEmployeeId, projectId);

            return Ok();
        }
    }
}
