using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace LTRegistrator.Domain.Entities
{
    public class User : IdentityUser
    {
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
