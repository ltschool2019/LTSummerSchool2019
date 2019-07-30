using System;

namespace LTRegistrator.Domain.Entities
{
    /// <summary>
    /// Describes many-to-many relationships between Department's and Employee's entities.
    /// </summary>
    public class DepartmentEmployee
    {
        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}