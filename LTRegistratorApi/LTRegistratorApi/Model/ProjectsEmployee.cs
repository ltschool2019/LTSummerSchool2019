using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    public class ProjectsEmployee
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }

        public Projects Projects { get; set; }
        public Employee Employee { get; set; }
        
    }
}
