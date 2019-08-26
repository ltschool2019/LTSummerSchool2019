using System;
using System.ComponentModel.DataAnnotations;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Basic information about the project.
    /// </summary>
    public class ProjectDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int TotalHours { get; set; }
    }
}
