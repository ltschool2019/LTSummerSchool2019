using System.Collections.Generic;


namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Describes employee entity.
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public string MaxRole { get; set; }
      
        public ICollection<ProjectEmployee> ProjectEmployee { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public ICollection<Leave> Leave { get; set; }
    }
}
