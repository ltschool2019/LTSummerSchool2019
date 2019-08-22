using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using LTRegistrator.BLL.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi
{
    /// <summary>
    /// Adding values ​​to a table.
    /// </summary>
    public class DbInitializer
    {
        public static void Initialize(DbContext context, UserManager<User> userManager)
        {
            context.Database.EnsureCreated();

            if (!context.Set<Employee>().Any())
            {
                var leaveBob = new Leave[]
                {
                    new Leave() { StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                    new Leave() { StartDate = new DateTime(2019, 8, 6), EndDate = new DateTime(2019, 8, 8), TypeLeave = TypeLeave.Vacation },
                    new Leave() { StartDate = new DateTime(2019, 8, 1), EndDate = new DateTime(2019, 8, 4), TypeLeave = TypeLeave.Vacation },
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

                context.Set<Employee>().Add(new Employee() { FirstName = "Alice", SecondName = "Brown", Mail = "alice@mail.ru", Rate = 1.5 });
                context.Set<Employee>().Add(new Employee() { FirstName = "Bob", SecondName = "Johnson", Mail = "b0b@yandex.ru", Leaves = leaveBob, Rate = 1 });
                context.Set<Employee>().Add(new Employee() { FirstName = "Eve", SecondName = "Williams", Mail = "eve.99@yandex.ru", Leaves = leaveEve, Rate = 1.25, ManagerId = 2 });
                context.Set<Employee>().Add(new Employee() { FirstName = "Carol", SecondName = "Smith", Mail = "car0l@mail.ru", Leaves = leaveCarol, Rate = 1 });
                context.Set<Employee>().Add(new Employee() { FirstName = "Dave", SecondName = "Jones", Mail = "dave.99@mail.ru", Rate = 1, ManagerId = 2 });
                context.Set<Employee>().Add(new Employee() { FirstName = "Frank", SecondName = "Florence", Mail = "frank.99@mail.ru", Leaves = leaveFrank, Rate = 0.25, ManagerId = 4 });

                context.SaveChanges();

                foreach (var employee in context.Set<Employee>())
                {
                    var user = new User
                    {
                        UserName = employee.FirstName + "_" + employee.SecondName,
                        Email = employee.Mail,
                        EmployeeId = employee.Id
                    };

                    var result = userManager.CreateAsync(user, employee.Mail + "Password1").Result;
                    //Retrieves the name of the constant in the specified enumeration that has the specified value.
                    var role = Enum.GetName(typeof(RoleType), employee.Mail == "alice@mail.ru" ? RoleType.Administrator : employee.ManagerId == null ? RoleType.Manager : RoleType.Employee);

                    var resultAddRole = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role)).Result;
                    if (!(result.Succeeded && resultAddRole.Succeeded))
                        throw new ApplicationException("ERROR_INITIALIZE_DB");
                }
            }

            if (!context.Set<Project>().Any())
            {
                context.Set<Project>().Add(new Project() { Name = "FOSS" });
                context.Set<Project>().Add(new Project() { Name = "EMIAS" });
                context.Set<Project>().Add(new Project() { Name = "Area 9" });
                context.SaveChanges();
            }

            if (!context.Set<ProjectEmployee>().Any())
            {
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 1 });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 2 });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 2 });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 3 });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 3 });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 3, EmployeeId = 3 });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 4 });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 5 });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 6 });

                context.SaveChanges();
            }
            if (!context.Set<Task>().Any())
            {
                context.Set<Task>().Add(new Task() { ProjectId = 1, EmployeeId = 1, Name = "FOSS" });
                context.Set<Task>().Add(new Task() { ProjectId = 1, EmployeeId = 2, Name = "FOSS" });
                context.Set<Task>().Add(new Task() { ProjectId = 2, EmployeeId = 2, Name = "EMIAS" });
                context.Set<Task>().Add(new Task() { ProjectId = 1, EmployeeId = 3, Name = "FOSS" });
                context.Set<Task>().Add(new Task() { ProjectId = 2, EmployeeId = 3, Name = "EMIAS" });
                context.Set<Task>().Add(new Task() { ProjectId = 3, EmployeeId = 3, Name = "Area 9" });
                context.Set<Task>().Add(new Task() { ProjectId = 2, EmployeeId = 4, Name = "EMIAS" });
                context.Set<Task>().Add(new Task() { ProjectId = 2, EmployeeId = 5, Name = "EMIAS" });
                context.SaveChanges();
            }

            if (!context.Set<TaskNote>().Any())
            {
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 1, Hours = 4, Day = new DateTime(2019, 1, 1) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 7, Day = new DateTime(2019, 8, 2) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 4, Day = new DateTime(2019, 8, 3) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 7, Day = new DateTime(2019, 8, 4) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 3, Hours = 8, Day = new DateTime(2019, 8, 1) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 3, Hours = 4, Day = new DateTime(2019, 8, 2) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 3, Hours = 7, Day = new DateTime(2019, 8, 3) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 4, Hours = 5, Day = new DateTime(2019, 7, 3) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 5, Hours = 8, Day = new DateTime(2019, 7, 3) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 6, Hours = 1, Day = new DateTime(2019, 7, 11) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 8, Hours = 7, Day = new DateTime(2019, 6, 11) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 8, Hours = 4, Day = new DateTime(2019, 7, 11) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 6, Day = new DateTime(2019, 7, 14) });

                context.SaveChanges();
            }
        }
    }
}
