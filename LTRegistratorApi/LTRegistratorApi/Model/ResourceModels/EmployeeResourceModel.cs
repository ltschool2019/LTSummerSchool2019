using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTRegistratorApi.Model.ResourceModels
{
    public class EmployeeResourceModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        public RoleTypeResourceModel MaxRole { get; set; }
        public double Rate { get; set; }

        public ICollection<ProjectResourceModel> Projects { get; set; }
    }
}
