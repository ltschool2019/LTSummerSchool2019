using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities.Base;

namespace LTRegistrator.Domain.Entities
{
    public class CustomFieldOptions: BaseEntity
    {
        public int Sequence { get; set; }
        public string CustomValue { get; set; }
    }
}
