using System.Collections.Generic;


namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Describes employee entity.
    /// </summary>
    public class Employee 
    {
        public int EmployeeID { get; set; }
        public string UserId { get; set; }
        public string User { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployee { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public ICollection<Project> Project { get; set; }
        public ICollection<Leave> Leave { get; set; }
    }
}
