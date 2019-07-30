using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using LTRegistrator.DAL;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;

namespace LTRegistratorApi
{
    /// <summary>
    /// Adding values ​​to a table.
    /// </summary>
    public class DbInitializer
    {
        public static void Initialize(LTRegistratorDbContext context, UserManager<User> userManager)
        {
            context.Database.EnsureCreated();

            if (!context.Employee.Any())
            {
                var leaveBob = new Leave[]
                {
                    new Leave() { StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                    new Leave() { StartDate = new DateTime(2019, 2, 10), EndDate = new DateTime(2019, 2, 13), TypeLeave = TypeLeave.SickLeave }
                };
                var leaveEve = new Leave[]
                {
                    new Leave() { StartDate = new DateTime(2019, 2, 10), EndDate = new DateTime(2019, 3, 1), TypeLeave = TypeLeave.Training }
                };
                var leaveCarol = new Leave[]
                {
                    new Leave() { StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                    new Leave() { StartDate = new DateTime(2019, 2, 1), EndDate = new DateTime(2019, 2, 15), TypeLeave = TypeLeave.Training },
                    new Leave() { StartDate = new DateTime(2019, 3, 1), EndDate = new DateTime(2019, 4, 1), TypeLeave = TypeLeave.SickLeave }
                };
                var leaveFrank = new Leave[]
                {
                    new Leave() { StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                    new Leave() { StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Idle }
                };

                AddEmployee(context, new Employee() { FirstName = "Alice", SecondName = "Brown", Mail = "alice@mail.ru", MaxRole = RoleType.Administrator, Rate = 1.5 });
                AddEmployee(context, new Employee() { FirstName = "Bob", SecondName = "Johnson", Mail = "b0b@yandex.ru", MaxRole = RoleType.Manager, Leaves = leaveBob, Rate = 1 });
                var newEmployee = new Employee { FirstName = "Eve", SecondName = "Williams", Mail = "eve.99@yandex.ru", MaxRole = RoleType.Employee, Leaves = leaveEve, Rate = 1.25 };
                AddEmployeeWithManager(context, "b0b@yandex.ru", newEmployee);
                AddEmployee(context, new Employee() { FirstName = "Carol", SecondName = "Smith", Mail = "car0l@mail.ru", MaxRole = RoleType.Manager, Leaves = leaveCarol, Rate = 1 });
                newEmployee = new Employee { FirstName = "Dave", SecondName = "Jones", Mail = "dave.99@mail.ru", MaxRole = RoleType.Employee, Rate = 1 };
                AddEmployeeWithManager(context, "b0b@yandex.ru", newEmployee);
                newEmployee = new Employee { FirstName = "Frank", SecondName = "Florence", Mail = "frank.99@mail.ru", MaxRole = RoleType.Employee, Leaves = leaveFrank, Rate = 0.25 };
                AddEmployeeWithManager(context, "car0l@mail.ru", newEmployee);

                context.SaveChanges();

                foreach (var employee in context.Employee)
                {
                    var user = new User
                    {
                        UserName = employee.FirstName + "_" + employee.SecondName,
                        Email = employee.Mail,
                        EmployeeId = employee.Id
                    };

                    var result = userManager.CreateAsync(user, employee.Mail + "Password1").Result;
                    //Retrieves the name of the constant in the specified enumeration that has the specified value.
                    var role = Enum.GetName(typeof(RoleType), employee.MaxRole);

                    var resultAddRole = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role)).Result;
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
                AddProjectEmployee(context, "A", "alice@mail.ru", RoleType.Employee);
                AddProjectEmployee(context, "A", "b0b@yandex.ru", RoleType.Manager);
                AddProjectEmployee(context, "B", "b0b@yandex.ru", RoleType.Employee);
                AddProjectEmployee(context, "A", "eve.99@yandex.ru", RoleType.Employee);
                AddProjectEmployee(context, "B", "eve.99@yandex.ru", RoleType.Employee);
                AddProjectEmployee(context, "C", "eve.99@yandex.ru", RoleType.Manager);
                AddProjectEmployee(context, "B", "car0l@mail.ru", RoleType.Manager);
                AddProjectEmployee(context, "B", "dave.99@mail.ru", RoleType.Employee);
                AddProjectEmployee(context, "B", "frank.99@mail.ru", RoleType.Employee);
            }
        }

        private static void AddEmployee(LTRegistratorDbContext context, Employee employee)
        {
            context.Employee.Add(employee);
            context.SaveChanges();
        }

        private static void AddEmployeeWithManager(LTRegistratorDbContext context, string managerEmail, Employee employee)
        {
            context.Employee.Add(employee);
            var manager = context.Employee.SingleOrDefault(e => e.Mail == managerEmail);
            if (manager != null)
            {
                employee.Manager = manager;
            };
            
            context.SaveChanges();
        }

        private static void AddProjectEmployee(LTRegistratorDbContext context, string projectName, string userEmail, RoleType roleType)
        {
            var projectEmployee = new ProjectEmployee()
            {
                Employee = context.Employee.SingleOrDefault(e => e.Mail == userEmail),
                Project = context.Project.SingleOrDefault(p => p.Name == projectName),
                Role = roleType
            };

            try
            {
                context.ProjectEmployee.Add(projectEmployee);
                context.SaveChanges();
            }
            catch
            {
                // ignored
            }
        }
    }
}
