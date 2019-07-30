using System;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Basic information about the project.
    /// </summary>
    public class ProjectDto
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
    }
}
