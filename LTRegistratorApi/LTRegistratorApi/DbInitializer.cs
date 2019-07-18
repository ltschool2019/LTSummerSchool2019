﻿using LTRegistratorApi.Model;
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
    /// <summary>
    /// Adding values ​​to a table.
    /// </summary>
    public class DbInitializer
    {
        public static void Initialize(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            context.Database.EnsureCreated();

            if (!context.Employee.Any())
            {
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
                context.Employee.Add(new Employee() { FirstName = "Alice", SecondName = "Brown", Mail = "alice@mail.ru", MaxRole = "Administrator" });
                context.Employee.Add(new Employee() { FirstName = "Bob", SecondName = "Johnson", Mail = "b0b@yandex.ru", MaxRole = "Manager", Leave = leaveEve });
                context.Employee.Add(new Employee() { FirstName = "Eve", SecondName = "Williams", Mail = "eve.99@yandex.ru", MaxRole = "Employee" });
                context.Employee.Add(new Employee() { FirstName = "Carol", SecondName = "Peterson", Mail = "car0l@mail.ru", MaxRole = "Manager", Leave = leaveCarol });
                context.Employee.Add(new Employee() { FirstName = "Dave", SecondName = "Finley", Mail = "dave.99@mail.ru", MaxRole = "Employee"});
                context.Employee.Add(new Employee() { FirstName = "Frank", SecondName = "Florence", Mail = "frank.99@mail.ru", MaxRole = "Employee", Leave = leaveFrank });

                context.SaveChanges();

                foreach (var employee in context.Employee)
                {
                    var user = new ApplicationUser
                    {
                        UserName = employee.Mail, //for PasswordSignInAsync
                        Email = employee.Mail,
                        EmployeeId = employee.EmployeeId
                    };

                    var result = userManager.CreateAsync(user, employee.Mail + "Password1").Result;
                    var resultAddRole = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, employee.MaxRole)).Result;
                    if (!(result.Succeeded && resultAddRole.Succeeded))
                        throw new ApplicationException("ERROR_INITIALIZE_DB");
                }
            }

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
