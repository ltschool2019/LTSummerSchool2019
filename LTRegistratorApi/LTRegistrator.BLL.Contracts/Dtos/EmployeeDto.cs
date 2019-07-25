using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.BLL.Contracts.Dtos
{
    public class EmployeeDto
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public string MaxRole { get; set; }

        public ICollection<EmployeeProjectEmployeeDto> ProjectEmployee { get; set; }
        public virtual EmployeeUserDto User { get; set; }
        public ICollection<EmployeeLeaveDto> Leave { get; set; }
    }

    public class EmployeeUserDto
    {
    }

    public class EmployeeLeaveDto
    {
        public TypeLeaveDto TypeLeave { get; set; }
        public DateTime StartDate { get; set; } //new DateTime(year, month, day);
        public DateTime EndDate { get; set; }
    }

    public class EmployeeProjectEmployeeDto
    {

    }

    public enum TypeLeaveDto
    {
        SickLeave, Vacation
    }
}
