using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTRegistrator.Domain.Entities;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IProjectService
    {
        Task<Response<Project>> GetProjectByIdAsync(int projectId);
        Task<Response<Project>> GetTemplateTypeByIdAsync(int projectId);
    }
}