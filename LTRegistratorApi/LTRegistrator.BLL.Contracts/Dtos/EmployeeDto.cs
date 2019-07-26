using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.BLL.Contracts.Dtos
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public string MaxRole { get; set; }

        public ICollection<ProjectEmployeeDto> ProjectEmployees { get; set; }
        public ICollection<LeaveDto> Leaves { get; set; }
    }

    public class ProjectEmployeeDto
    {
        public Guid ProjectId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Role { get; set; }

        public ProjectDto Project { get; set; }
    }

    public enum TypeLeaveDto
    {
        SickLeave, Vacation
    }
}
