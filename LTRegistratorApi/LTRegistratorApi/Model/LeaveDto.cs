using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTRegistratorApi.Validators;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LTRegistratorApi.Model
{
    public class LeaveDto
    {
        public int Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TypeLeaveDto TypeLeave { get; set; }
        [LeaveDate]
        public DateTime StartDate { get; set; }
        [LeaveDate(nameof(StartDate))]
        public DateTime EndDate { get; set; }
    }
}
