using System;
using System.Collections.Generic;


namespace LTRegistratorApi.Model
{
    public enum RoleType { Employee, Manager, Administrator };

    /// <summary>
    /// Describes employee entity.
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleType MaxRole { get; set; }
        private double rate;
        public double Rate
        {
            get { return rate; }
            set
            {
                if (value != 0.25 && value != 0.5 && value != 0.75 && value != 1 && value != 1.25 && value != 1.5)
                    throw new ApplicationException("INVALID_RATE");

                rate = value;
            }
        }

        public ICollection<ProjectEmployee> ProjectEmployee { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public ICollection<Leave> Leaves { get; set; }
    }
}
