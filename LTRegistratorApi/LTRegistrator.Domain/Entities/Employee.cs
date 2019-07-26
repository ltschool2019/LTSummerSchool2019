using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities.Base;

namespace LTRegistrator.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public string MaxRole { get; set; }

        public Guid UserId { get; set; }

        public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Leave> Leaves { get; set; }
    }
}
