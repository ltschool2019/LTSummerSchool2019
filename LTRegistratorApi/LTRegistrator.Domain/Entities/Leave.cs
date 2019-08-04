using System;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Entities.Base;
using LTRegistrator.Domain.Enums;

namespace LTRegistrator.Domain.Entities
{
    /// <summary>
    /// Describes employee leave.
    /// </summary>
    public class Leave : BaseEntity
    {
        public TypeLeave TypeLeave { get; set; }
        public DateTime StartDate { get; set; } //new DateTime(year, month, day);
        public DateTime EndDate { get; set; }

        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
