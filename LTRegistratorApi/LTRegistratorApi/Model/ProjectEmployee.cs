using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    /// <summary>
    ///this class created in the aim of configuration relationship many-to-many between Project's and Employee's entities
    /// </summary>
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }

        public Project Project { get; set; }
        public Employee Employee { get; set; }
        
    }
}
