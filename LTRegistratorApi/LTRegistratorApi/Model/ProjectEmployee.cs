using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    public enum RoleType { Employee, Manager, Administrator};
    /// <summary>
    ///Describes many-to-many relationships between Project's and Employee's entities.
    /// </summary>
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public RoleType RoleType { get; set; }

        public Project Project { get; set; }
        public Employee Employee { get; set; }

    }
}
