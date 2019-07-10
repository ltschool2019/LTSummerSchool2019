using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LTRegistratorApi.Model
{
    public class Employee 
    {
        public int EmployeeID { get; set; }
        public string UserId { get; set; }
        public string User { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployee { get; set; }


        public virtual UserApplication UserApplication { get; set; }
    }
}
