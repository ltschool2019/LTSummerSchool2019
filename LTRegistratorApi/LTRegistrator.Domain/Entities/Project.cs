using System.Collections.Generic;
using LTRegistrator.Domain.Entities.Base;

namespace LTRegistrator.Domain.Entities
{
    /// <summary>
    /// Describes project entity.
    /// </summary>
    public class Project : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }
    }
}
