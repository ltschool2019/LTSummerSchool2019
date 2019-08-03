using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using LTRegistrator.BLL.Services;
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

                context.Employee.Add(new Employee() { FirstName = "Alice", SecondName = "Brown", Mail = "alice@mail.ru", MaxRole = RoleType.Administrator, Rate = 1.5 });
                context.Employee.Add(new Employee() { FirstName = "Bob", SecondName = "Johnson", Mail = "b0b@yandex.ru", MaxRole = RoleType.Manager, Leaves = leaveBob, Rate = 1 });
                context.Employee.Add(new Employee() { FirstName = "Eve", SecondName = "Williams", Mail = "eve.99@yandex.ru", MaxRole = RoleType.Employee, Leaves = leaveEve, Rate = 1.25, ManagerId = 2 });
                context.Employee.Add(new Employee() { FirstName = "Carol", SecondName = "Smith", Mail = "car0l@mail.ru", MaxRole = RoleType.Manager, Leaves = leaveCarol, Rate = 1 });
                context.Employee.Add(new Employee() { FirstName = "Dave", SecondName = "Jones", Mail = "dave.99@mail.ru", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 2 });
                context.Employee.Add(new Employee() { FirstName = "Frank", SecondName = "Florence", Mail = "frank.99@mail.ru", MaxRole = RoleType.Employee, Leaves = leaveFrank, Rate = 0.25, ManagerId = 4 });

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
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 1, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 2, Role = RoleType.Manager });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 2, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 3, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 3, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 3, EmployeeId = 3, Role = RoleType.Manager });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 4, Role = RoleType.Manager });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 5, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 6, Role = RoleType.Employee });

                context.SaveChanges();
            }
            if (!context.Task.Any())
            {
                context.Task.Add(new Task() { ProjectId = 1, EmployeeId = 1, Name = "Create database"});
                context.Task.Add(new Task() { ProjectId = 1, EmployeeId = 2, Name = "Сreate registration method" });
                context.Task.Add(new Task() { ProjectId = 1, EmployeeId = 2, Name = "Сreate login method" });
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 2, Name = "Сreate login method" });
                context.Task.Add(new Task() { ProjectId = 1, EmployeeId = 3, Name = "Create AccountController"});
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 3, Name = "Create ManagerController"});
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 3, Name = "Create EmployeeController" });
                context.Task.Add(new Task() { ProjectId = 3, EmployeeId = 3, Name = "Configure authentication for different roles"});
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 4, Name = "Create layouts for the client application"});
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 5, Name = "Prepare for presentation" });
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 6, Name = "Fix DeleteLeaves method"});
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 6, Name = "Сreate login method" });

                context.SaveChanges();
            }

            if (!context.TaskNote.Any())
            {
                context.TaskNote.Add(new TaskNote() { TaskId = 1, Houre = 4, Day = new DateTime(2019, 1, 1) });
                context.TaskNote.Add(new TaskNote() { TaskId = 2, Houre = 7, Day = new DateTime(2019, 8, 2) });
                context.TaskNote.Add(new TaskNote() { TaskId = 2, Houre = 4, Day = new DateTime(2019, 8, 3) });
                context.TaskNote.Add(new TaskNote() { TaskId = 2, Houre = 7, Day = new DateTime(2019, 8, 4) });
                context.TaskNote.Add(new TaskNote() { TaskId = 3, Houre = 8, Day = new DateTime(2019, 8, 1) });
                context.TaskNote.Add(new TaskNote() { TaskId = 3, Houre = 4, Day = new DateTime(2019, 8, 2) });
                context.TaskNote.Add(new TaskNote() { TaskId = 3, Houre = 7, Day = new DateTime(2019, 8, 3) });
                context.TaskNote.Add(new TaskNote() { TaskId = 4, Houre = 5, Day = new DateTime(2019, 7, 3) });
                context.TaskNote.Add(new TaskNote() { TaskId = 5, Houre = 8, Day = new DateTime(2019, 7, 3) });
                context.TaskNote.Add(new TaskNote() { TaskId = 6, Houre = 1, Day = new DateTime(2019, 7, 11) });                
                context.TaskNote.Add(new TaskNote() { TaskId = 10, Houre = 7, Day = new DateTime(2019, 6, 11) });
                context.TaskNote.Add(new TaskNote() { TaskId = 11, Houre = 4, Day = new DateTime(2019, 7, 11) });
                context.TaskNote.Add(new TaskNote() { TaskId = 12, Houre = 6, Day = new DateTime(2019, 7, 14) });

                context.SaveChanges();
            }
        }
    }
}
