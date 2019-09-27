using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.Domain.Entities
{
    public class CustomFieldProject
    {
        public int ProjectId { get; set; }
        public int CustomFieldId { get; set; }

        public virtual Project Project { get; set; }
        public virtual CustomField CustomField { get; set; }
    }
}
