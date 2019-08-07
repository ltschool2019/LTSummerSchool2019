using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    public class TaskNoteDto
    {
        public DateTime Day { get; set; }
        public int Hours { get; set; }
    }
}
