using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTRegistratorApi.Validators;

namespace LTRegistratorApi.Model
{
    public class LeaveInputDto
    {
        public TypeLeaveDto TypeLeave { get; set; }
        [LeaveDate]
        public DateTime StartDate { get; set; }
        [LeaveDate(nameof(StartDate))]
        public DateTime EndDate { get; set; }
    }
}
