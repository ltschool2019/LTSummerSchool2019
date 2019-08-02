using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Basic information about the employee.
    /// </summary>
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mail { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public RoleTypeDto MaxRole { get; set; }
        public double Rate { get; set; }
        public int? ManagerId { get; set; }


        public ICollection<ProjectDto> Projects { get; set; }
    }
}
