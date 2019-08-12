using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTRegistratorApi.Validators;

namespace LTRegistratorApi.Model
{
    public class TaskInputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TaskNoteDto> TaskNotes { get; set; }
    }
}
