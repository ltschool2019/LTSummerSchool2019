using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    public class Employee
    {
        //EmployeeID, UserId, User - из AspNetUsers таблицы, List <ProjectsIds>
        public int EmployeeID { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }

        public ICollection<ProjectsEmployee> ProjectsEmployee { get; set; }
    }
}
