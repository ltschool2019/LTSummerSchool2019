using System.Collections.Generic;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Describes department entity.
    /// </summary>
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }

        public ICollection<DepartmentEmployee> DepartmentEmployee { get; set; }
    }
}