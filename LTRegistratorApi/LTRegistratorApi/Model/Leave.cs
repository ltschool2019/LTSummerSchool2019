using System;

namespace LTRegistratorApi.Model
{
    
    /// <summary>
    /// Describes employee leave.
    /// </summary>
    public class Leave
    {
        public int LeaveId { get; set; }

        public enum TypeLeave { SickLeave, Vacation };
        public DateTime StartDate { get; set; } //new DateTime(year, month, day);
        public DateTime EndDate { get; set; }
    }
}
