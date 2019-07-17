using System;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Describes employee leave.
    /// </summary>
    public class Leave
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public enum TypeLeave { SickLeave, Vacation };
        public static DateTime StartDate { get; set; } //new DateTime(year, month, day);
        public static DateTime EndDate { get; set; }
    }
}
