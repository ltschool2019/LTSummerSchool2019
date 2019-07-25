namespace LTRegistratorApi.Model
{
    public enum RoleType { Employee, Manager, Administrator};
    /// <summary>
    /// Describes many-to-many relationships between Project's and Employee's entities.
    /// </summary>
    public class ProjectEmployee
    {
        public RoleType Role { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
