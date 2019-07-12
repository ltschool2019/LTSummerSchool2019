using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LTRegistratorApi.Model
{
    //This additional class which created in the aim of relationships to AspNetUsers
    public class ApplicationUser : IdentityUser
    {
        public virtual Employee Employee { get; set; }
        public int EmployeeId { get; set; }
    }
}
