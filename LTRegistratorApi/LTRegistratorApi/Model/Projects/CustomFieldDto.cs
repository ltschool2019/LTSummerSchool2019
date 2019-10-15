using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTRegistratorApi.Model.Enums;

namespace LTRegistratorApi.Model.Projects
{
    public class CustomFieldDto
    {
        public int Id { get; set; }
        public CustomFieldType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public string DefaultValue { get; set; } 
        public int MaxLength { get; set; }
        public IEnumerable<CustomFieldOptionDto> FieldOptions { get; set; }
    }
}
