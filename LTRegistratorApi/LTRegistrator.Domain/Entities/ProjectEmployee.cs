using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.Domain.Entities
{
    public class ProjectEmployee
    {
        public Guid ProjectId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Role { get; set; }

        public virtual Project Project { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
