using System;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Basic information about the project.
    /// </summary>
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalHours { get; set; }
    }
}
