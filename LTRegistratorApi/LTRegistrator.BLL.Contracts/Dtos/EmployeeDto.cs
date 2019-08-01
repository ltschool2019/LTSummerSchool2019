using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.BLL.Contracts.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleTypeDto MaxRole { get; set; }
        public double Rate { get; set; }

        public ICollection<ProjectEmployeeDto> ProjectEmployees { get; set; }
        public ICollection<LeaveDto> Leaves { get; set; }
        public ManagerDto Manager { get; set; }
    }

    public class ManagerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleTypeDto MaxRole { get; set; }
        public double Rate { get; set; }
    }

    public class ProjectEmployeeDto
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string Role { get; set; }

        public ProjectDto Project { get; set; }
    }
    public enum TypeLeaveDto
    {
        SickLeave, Vacation, Training, Idle
    }

    public enum RoleTypeDto
    {
        Employee, Manager, Administrator
    }
}
