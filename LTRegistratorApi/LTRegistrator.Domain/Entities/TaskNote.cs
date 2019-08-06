using System;
using System.Collections.Generic;
using LTRegistrator.Domain.Entities.Base;

namespace LTRegistrator.Domain.Entities
{
    public class TaskNote : BaseEntity
    {
        public DateTime Day { get; set; }
        public int Hours { get; set; }

        public int TaskId { get; set; }
        public virtual Task Task { get; set; }

    }
}