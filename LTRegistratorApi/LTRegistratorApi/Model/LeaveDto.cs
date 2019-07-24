using System;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Basic information about leave.
    /// </summary>
    public class LeaveDto
    {
        public TypeLeave TypeLeave { get; set; }
        public DateTime StartDate { get; set; } //new DateTime(year, month, day);
        public DateTime EndDate { get; set; }
    }
}
