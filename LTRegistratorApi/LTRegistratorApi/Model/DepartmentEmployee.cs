namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Describes many-to-many relationships between Department's and Employee's entities.
    /// </summary>
    public class DepartmentEmployee
    {
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}