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
    public class UserApplication : IdentityUser
    {
        public virtual Employee Employee { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
