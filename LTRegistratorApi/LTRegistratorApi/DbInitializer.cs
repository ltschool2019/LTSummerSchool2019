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
                new RegisterDto { Name = "Eve", Email = "eve.99@yandex.ru", Password = "1Adam!!!", Role = "Employee" },
                new RegisterDto { Name = "Carol", Email = "car0l@mail.ru", Password = "+C0o0C+", Role = "Manager" },
                new RegisterDto { Name = "Dave", Email = "dave.99@mail.ru", Password = "1Adam!!!", Role = "Employee" },
                new RegisterDto { Name = "Frank", Email = "frank.99@mail.ru", Password = "1Adam!!!", Role = "Employee" }
            };

            if (context.Users.Count() == 0)
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

            var leaveEve = new Leave[]
            {
                new Leave() { StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                new Leave() { StartDate = new DateTime(2019, 2, 10), EndDate = new DateTime(2019, 2, 13), TypeLeave = TypeLeave.SickLeave }
            };
            var leaveCarol = new Leave[]
            {
                new Leave() { StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                new Leave() { StartDate = new DateTime(2019, 3, 1), EndDate = new DateTime(2019, 4, 1), TypeLeave = TypeLeave.SickLeave }
            };
            var leaveFrank = new Leave[]
            {
                new Leave() { StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation }
            };

            if (!context.Employee.Any())
            {
                context.Employee.Add(new Employee() { User = "Alice" });
                context.Employee.Add(new Employee() { User = "Eve", Leave = leaveEve });
                context.Employee.Add(new Employee() { User = "Bob" });
                context.Employee.Add(new Employee() { User = "Carol", Leave = leaveCarol });
                context.Employee.Add(new Employee() { User = "Dave" });
                context.Employee.Add(new Employee() { User = "Frank", Leave = leaveFrank });

                context.SaveChanges();
            }
        }
    }
}
