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

            //var users = new RegisterDto[]
            //{
            //    new RegisterDto { Name = "Alice", Email = "alice@mail.ru", Password = "aA123456!", Role = "Administrator" },
            //    new RegisterDto { Name = "Bob", Email = "b0b@yandex.ru", Password = "+B0o0B+", Role = "Manager" },
            //    new RegisterDto { Name = "Eve", Email = "eve.99@yandex.ru", Password = "1Adam!!!", Role = "Employee" }
            //};

            //if (context.Users.Count() == 0)
            //{
            //    foreach (var model in users)
            //    {
            //        var user = new ApplicationUser
            //        {
            //            UserName = model.Email, //for PasswordSignInAsync
            //            Email = model.Email,
            //        };

            //        var result = userManager.CreateAsync(user, model.Password).Result;
            //        var resultAddRole = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, model.Role)).Result;
            //        var resultAddName = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, model.Name)).Result;

            //        if (!(result.Succeeded && resultAddRole.Succeeded && resultAddName.Succeeded))
            //            throw new ApplicationException("ERROR_INITIALIZE_DB");
            //    }
            //}



            if (!context.Employee.Any())
            {
                context.Employee.Add(new Employee() { FirstName = "Bob", SecondName = "Johnson", Mail = "b0b@yandex.ru", MaxRole = "Manager" });
                context.Employee.Add(new Employee() { FirstName = "Eve", SecondName = "Williams", Mail = "eve.99@yandex.ru", MaxRole = "Employee" });
                context.Employee.Add(new Employee() { FirstName = "Alice", SecondName = "Brown", Mail = "alice@mail.ru", MaxRole = "Administrator" });
                context.SaveChanges();
            }

            //foreach (var employee in context.Employee)
            //{
            //    var user = new ApplicationUser
            //    {
            //        UserName = employee.FirstName, //for PasswordSignInAsync
            //        Email = employee.Mail,
            //        EmployeeId = employee.EmployeeId
            //    };

            //    var result = userManager.CreateAsync(user, employee.FirstName + "xx").Result;
            //    var resultAddRole = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, employee.MaxRole)).Result;
            //    if (!(result.Succeeded && resultAddRole.Succeeded))
            //        throw new ApplicationException("ERROR_INITIALIZE_DB");
            //}

            if (!context.Project.Any())
            {
                context.Project.Add(new Project() { Name = "A" });
                context.Project.Add(new Project() { Name = "B" });
                context.Project.Add(new Project() { Name = "С" });
                context.SaveChanges();
            }

            if (!context.ProjectEmployee.Any())
            {
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 1, Role = "Manager" });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 3, EmployeeId = 1, Role = "Manager" });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 1, Role = "Employee" });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 2, Role = "Employee" });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 2, Role = "Employee" });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 3, Role = "Employee" });
                context.SaveChanges();
            }

           



        }
    }
}
