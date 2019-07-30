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
        public RoleTypeDto MaxRole { get; set; }
        public double Rate { get; set; }

        public ICollection<ProjectEmployeeDto> ProjectEmployees { get; set; }
        public ICollection<LeaveDto> Leaves { get; set; }
        public ManagerDto Manager { get; set; }
        public ICollection<DepartmentEmployeesDto> DepartmentEmployees { get; set; }
    }

    public class ManagerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleTypeDto MaxRole { get; set; }
        public double Rate { get; set; }
    }

    public class ProjectEmployeeDto
    {
        public Guid ProjectId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Role { get; set; }

        public ProjectDto Project { get; set; }
    }

    public class DepartmentEmployeesDto
    {
        public Guid DepartmentId { get; set; }
        public Guid EmployeeId { get; set; }

        public DepartmentDto Department { get; set; }
    }

    public class DepartmentDto
    {
        public string Name { get; set; }
    }

    public enum TypeLeaveDto
    {
        SickLeave, Vacation
    }

    public enum RoleTypeDto
    {
        Employee, Manager, Administrator
    }
}
