using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }

        public Project Project { get; set; }
        public Employee Employee { get; set; }
        
    }
}
