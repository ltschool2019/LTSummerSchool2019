using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Describes project entity.
    /// </summary>
    public class Project
    {

        public int ProjectId { get; set; }

        public string Name { get; set; }
        public int ManagerId { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployee { get; set; }
        public virtual Employee Employee { get; set; }
    }
}

