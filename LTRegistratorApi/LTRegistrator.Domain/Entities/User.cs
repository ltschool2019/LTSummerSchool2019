using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace LTRegistrator.Domain.Entities
{
    public class User : IdentityUser<Guid>, IGuidId
    {
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
