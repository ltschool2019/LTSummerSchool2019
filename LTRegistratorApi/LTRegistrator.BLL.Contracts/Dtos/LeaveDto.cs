using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.BLL.Contracts.Dtos
{
    public class LeaveDto
    {
        public Guid Id { get; set; }
        public TypeLeaveDto TypeLeave { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid EmployeeId { get; set; }
    }
}
