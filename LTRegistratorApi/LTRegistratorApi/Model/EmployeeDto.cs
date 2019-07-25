using System.Collections.Generic;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Basic information about the employee.
    /// </summary>
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleType MaxRole { get; set; }
        public double Rate { get; set; }

        public ICollection<ProjectDto> Projects { get; set; }
    }
}
