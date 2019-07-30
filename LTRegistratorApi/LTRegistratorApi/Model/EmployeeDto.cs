using System;
using System.Collections.Generic;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Basic information about the employee.
    /// </summary>
    public class EmployeeDto
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleType MaxRole { get; set; }
        public double Rate { get; set; }
        public Guid? ManagerId { get; set; }


        public ICollection<ProjectDto> Projects { get; set; }
    }
}
