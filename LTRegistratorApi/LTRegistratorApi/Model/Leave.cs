using System;

namespace LTRegistratorApi.Model
{
    public enum TypeLeave { SickLeave, Vacation };
    /// <summary>
    /// Describes employee leave.
    /// </summary>
    public class Leave
    {
        public int LeaveId { get; set; }
        public TypeLeave TypeLeave { get; set; }
        public DateTime StartDate { get; set; } //new DateTime(year, month, day);
        public DateTime EndDate { get; set; }
    }
}
