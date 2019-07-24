using System.Collections.Generic;


namespace LTRegistratorApi.Model
{
    public enum RoleType { Employee, Manager, Administrator };
    /// <summary>
    /// Describes employee entity.
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleType MaxRole { get; set; }
      
        public ICollection<ProjectEmployee> ProjectEmployee { get; set; }
        public ICollection<DepartmentEmployee> DepartmentEmployee { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public ICollection<Leave> Leaves { get; set; }
    }
}
