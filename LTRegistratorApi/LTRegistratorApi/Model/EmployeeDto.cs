using System.Collections.Generic;

namespace LTRegistratorApi.Model
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public string MaxRole { get; set; }

        public ICollection<ProjectDto> Projects { get; set; }
    }
}
