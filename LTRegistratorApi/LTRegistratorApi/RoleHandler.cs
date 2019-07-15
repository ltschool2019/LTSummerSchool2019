using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LTRegistratorApi
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        /// <summary>
        /// This method is called when trying to access a resource to which the restriction applies.
        /// </summary>
        /// <param name="context">Authorization context containing information about the request</param>
        /// <param name="requirement">The object of the restriction applied</param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            RoleRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                string role = context.User.FindFirst(c => c.Type == ClaimTypes.Role).Value;
                if (role == requirement.Role)
                {
                    //Inherited method, called if the request matches the restriction
                    //If this method is not called, it is considered that the authorization failed.
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
