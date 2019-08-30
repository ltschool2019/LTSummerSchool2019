using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class ProjectService : BaseService, IProjectService
    {
        public ProjectService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }
        public async Task<Response<Project>> GetProjectByIdAsync(int projectId)
        {
            var project = await DbContext.Set<Project>().FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                return new Response<Project>(HttpStatusCode.NotFound, $"Project with id = {projectId} not found");
            }
            return new Response<Project>(project);
        }
        public async Task<Response<Project>> GetTemplateTypeByIdAsync(int projectId)
        {
            var templateTypeProject = await DbContext.Set<Project>().FirstOrDefaultAsync(p => p.TemplateType == TemplateType.HoursPerProject && p.Id == projectId);           
            if (templateTypeProject == null)
            {
                return new Response<Project>(HttpStatusCode.NotFound, $"Project with id = {projectId} not found or template type is not correct");
            }
            return new Response<Project>(templateTypeProject);
        }
    }
}
