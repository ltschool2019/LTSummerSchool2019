using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTRegistratorApi.Validators;

namespace LTRegistratorApi.Model.ResourceModels
{
    public class LeaveResourceModel
    {
        public int Id { get; set; }
        public TypeLeaveResourceModel TypeLeave { get; set; }
        [LeaveDate]
        public DateTime StartDate { get; set; }
        [LeaveDate("StartDate")]
        public DateTime EndDate { get; set; }
    }

    public enum TypeLeaveResourceModel
    {
        SickLeave, Vacation, Training, Idle
    }
}
