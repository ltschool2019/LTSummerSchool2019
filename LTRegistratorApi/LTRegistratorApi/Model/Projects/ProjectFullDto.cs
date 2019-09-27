using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model.Projects
{
    public class ProjectFullDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<CustomFieldDto> CustomFields { get; set; }
    }
}
