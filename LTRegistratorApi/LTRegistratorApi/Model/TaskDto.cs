using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    public class TaskDto
    {
        public string Name { get; set; }
        public List<TaskNoteDto> TaskNotes { get; set; }
        public List<LeaveDto> Leave {get;set;}
    }
}
