using System.Collections.Generic;
using LTRegistrator.Domain.Entities.Base;
using LTRegistrator.Domain.Enums;

namespace LTRegistrator.Domain.Entities
{
    /// <summary>
    /// Describes project entity.
    /// </summary>
    public class Project : BaseEntity
    {
        public string Name { get; set; }
        public TemplateType TemplateType { get; set; }
        public bool SoftDeleted { get; set; } = false;

        public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }
    }
}
