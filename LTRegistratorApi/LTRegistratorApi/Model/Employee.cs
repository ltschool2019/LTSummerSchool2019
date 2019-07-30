using System;
using System.Collections.Generic;
using System.Linq;

namespace LTRegistratorApi.Model
{
    public enum RoleType { Employee, Manager, Administrator };

    /// <summary>
    /// Describes employee entity.
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int? ManagerId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleType MaxRole { get; set; }
        private static readonly double[] ValidRateValues = new[] { 0.25, 0.5, 0.75, 1, 1.25, 1.5 };
        private double rate;
        public double Rate
        {
            get { return rate; }
            set
            {
                if (!ValidRateValues.Contains(value))
                    throw new ApplicationException("INVALID_RATE");
                rate = value;
            }
        }

        public ICollection<ProjectEmployee> ProjectEmployee { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Leave> Leaves { get; set; }
        public Employee Manager { get; set; }
    }
}
