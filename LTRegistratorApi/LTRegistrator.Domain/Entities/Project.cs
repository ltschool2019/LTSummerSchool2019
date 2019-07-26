using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities.Base;

namespace LTRegistrator.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }
    }
}
