using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LTRegistratorApi
{
    /// <summary>
    /// The class of restriction sets the required role for access
    /// </summary>
    public class RoleRequirement : IAuthorizationRequirement
    {
        protected internal string Role { get; set; }

        public RoleRequirement(string role)
        {
            Role = role;
        }
    }
}
