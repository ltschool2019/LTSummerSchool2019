using System.Collections.Generic;
using LTRegistrator.Domain.Entities.Base;

namespace LTRegistrator.Domain.Entities
{
    /// <summary>
    /// Describes department entity.
    /// </summary>
    public class Department : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<DepartmentEmployee> DepartmentEmployee { get; set; }
    }
}