using System;
using System.Collections.Generic;
using System.Linq;
using LTRegistrator.Domain.Entities.Base;
using LTRegistrator.Domain.Enums;

namespace LTRegistrator.Domain.Entities
{
    /// <summary>
    /// Describes employee entity.
    /// </summary>
    public class Employee : BaseEntity
    {
        public int? ManagerId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleType MaxRole { get; set; }
        public double Rate { get; set; }

        public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Leave> Leaves { get; set; }
        public virtual Employee Manager { get; set; }
    }
}
