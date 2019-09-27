using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTRegistratorApi.Model.CustomValues;

namespace LTRegistratorApi.Model.Tasks
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TaskNoteDto> TaskNotes { get; set; }
        public IEnumerable<CustomValueDto> CustomValues { get; set; }
        public List<LeaveDto> Leave { get; set; }
    }
}
