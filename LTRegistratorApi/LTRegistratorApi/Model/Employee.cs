using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }

        public ICollection<ProjectEmployee> ProjectEmployee { get; set; }
    }
}
