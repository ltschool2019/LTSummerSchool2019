using System.Collections.Generic;
using LTRegistrator.Domain.Entities.Base;

namespace LTRegistrator.Domain.Entities
{
    /// <summary>
    /// Describes an entity with project tasks.
    /// </summary>
    public class Task : BaseEntity
    {
        public string Name { get; set; }

        public int ProjectId { get; set; }       
        public int EmployeeId { get; set; }
        public virtual ProjectEmployee ProjectEmployee { get; set; }

        public virtual ICollection<TaskNote> TaskNotes { get; set; }

    }
}