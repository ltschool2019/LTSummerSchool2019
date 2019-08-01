using System;
using LTRegistrator.Domain.Enums;

namespace LTRegistrator.Domain.Entities
{
    /// <summary>
    /// Describes many-to-many relationships between Project's and Employee's entities.
    /// </summary>
    public class ProjectEmployee
    {
        public RoleType Role { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}