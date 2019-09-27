using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities.Base;
using LTRegistrator.Domain.Enums;

namespace LTRegistrator.Domain.Entities
{
    public class CustomField: BaseEntity
    {
        public CustomFieldType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public string DefaultValue { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        public virtual ICollection<CustomValue> CustomValues { get; set; }
        public virtual ICollection<CustomFieldProject> CustomFieldProjects { get; set; }
        public virtual ICollection<CustomFieldOption> CustomFieldOptions { get; set; }
    }
}
