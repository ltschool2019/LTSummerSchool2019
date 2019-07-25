using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities.Base;
using LTRegistrator.Domain.Enums;

namespace LTRegistrator.Domain.Entities
{
    public class Leave : BaseEntity
    {
        public TypeLeave TypeLeave { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
