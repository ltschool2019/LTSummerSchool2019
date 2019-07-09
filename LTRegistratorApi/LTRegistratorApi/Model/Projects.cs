using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    public class Projects
    {
        // Name, ProjectId, ManagerId,Manager, List<EmployeeId>
        public int ProjectId { get; set; }

        public string Name { get; set; }
        public int ManagerId { get; set; }
        public string Manager { get; set; }
        public ICollection<ProjectsEmployee> ProjectsEmployee { get; set; }



    }
}
