using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.Domain.Entities
{
    public class CustomValue
    {
        public int CustomFieldId { get; set; }
        public int TaskId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public virtual CustomField CustomField { get; set; }
        public virtual Task Task { get; set; }
    }
}
