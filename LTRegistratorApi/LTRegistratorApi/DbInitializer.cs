using LTRegistratorApi.Model;
using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LTRegistratorApi
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            context.Database.EnsureCreated();

            var users = new RegisterDto[]
            {
                new RegisterDto { Name = "Alice", Email = "alice@mail.ru", Password = "aA123456!", Role = "Administrator" },
                new RegisterDto { Name = "Bob", Email = "b0b@yandex.ru", Password = "+B0o0B+", Role = "Manager" },
                new RegisterDto { Name = "Eve", Email = "eve.99@yandex.ru", Password = "1Adam!!!", Role = "Employee" }
            };

            if (context.Users.Any())
            {
                foreach (var model in users)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email, //for PasswordSignInAsync
                        Email = model.Email,
                    };

                    var result = userManager.CreateAsync(user, model.Password).Result;
                    var resultAddRole = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, model.Role)).Result;
                    var resultAddName = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, model.Name)).Result;

                    if (!(result.Succeeded && resultAddRole.Succeeded && resultAddName.Succeeded))
                        throw new ApplicationException("ERROR_INITIALIZE_DB");
                }
            }
        }
    }
}
