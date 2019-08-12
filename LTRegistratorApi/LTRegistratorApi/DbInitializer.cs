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

                context.Employee.Add(new Employee() { FirstName = "Антон", SecondName = "Сапановский", Mail = "anvitsapik@gmail.com", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 2 });
                context.Employee.Add(new Employee() { FirstName = "Сергей", SecondName = "Сергеев", Mail = "seregak@gmail.com", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 2 });
                context.Employee.Add(new Employee() { FirstName = "Виталий", SecondName = "Витальев", Mail = "vitaly@gmail.com", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 2 });
                context.Employee.Add(new Employee() { FirstName = "Николай", SecondName = "Николаев", Mail = "niko@gmail.com", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 2 });
                context.Employee.Add(new Employee() { FirstName = "Юлия", SecondName = "Юлькина", Mail = "uliya@gmail.com", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 2 });
                context.Employee.Add(new Employee() { FirstName = "Александра", SecondName = "Сашкова", Mail = "sashkovak@gmail.com", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 2 });
                context.Employee.Add(new Employee() { FirstName = "Павел", SecondName = "Пашков", Mail = "pahacool@gmail.com", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 2 });

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
                context.Project.Add(new Project() { Name = "FOSS" });
                context.Project.Add(new Project() { Name = "EMIAS" });
                context.Project.Add(new Project() { Name = "Area 9" });
                context.Project.Add(new Project() { Name = "EdMaven" });
                context.Project.Add(new Project() { Name = "InOut" });
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

                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 4, EmployeeId = 7, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 4, EmployeeId = 8, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 4, EmployeeId = 9, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 4, EmployeeId = 10, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 4, EmployeeId = 11, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 4, EmployeeId = 12, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 5, EmployeeId = 7, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 5, EmployeeId = 8, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 5, EmployeeId = 9, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 5, EmployeeId = 10, Role = RoleType.Employee });
                context.ProjectEmployee.Add(new ProjectEmployee() { ProjectId = 5, EmployeeId = 12, Role = RoleType.Employee });

                context.SaveChanges();
            }
            if (!context.Task.Any())
            {
                context.Task.Add(new Task() { ProjectId = 1, EmployeeId = 1, Name = "FOSS"});
                context.Task.Add(new Task() { ProjectId = 1, EmployeeId = 2, Name = "FOSS"});
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 2, Name = "EMIAS"});
                context.Task.Add(new Task() { ProjectId = 1, EmployeeId = 3, Name = "FOSS"});
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 3, Name = "EMIAS"});
                context.Task.Add(new Task() { ProjectId = 3, EmployeeId = 3, Name = "Area 9"});
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 4, Name = "EMIAS"});
                context.Task.Add(new Task() { ProjectId = 2, EmployeeId = 5, Name = "EMIAS"});

                context.Task.Add(new Task() { ProjectId = 4, EmployeeId = 7, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 4, EmployeeId = 8, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 4, EmployeeId = 9, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 4, EmployeeId = 10, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 4, EmployeeId = 11, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 4, EmployeeId = 12, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 5, EmployeeId = 7, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 5, EmployeeId = 8, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 5, EmployeeId = 9, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 5, EmployeeId = 10, Name = "SomeTask" });
                context.Task.Add(new Task() { ProjectId = 5, EmployeeId = 12, Name = "SomeTask" });

                context.SaveChanges();
            }

            if (!context.TaskNote.Any())
            {
                context.TaskNote.Add(new TaskNote() { TaskId = 1, Hours = 4, Day = new DateTime(2019, 1, 1) });
                context.TaskNote.Add(new TaskNote() { TaskId = 2, Hours = 7, Day = new DateTime(2019, 8, 2) });
                context.TaskNote.Add(new TaskNote() { TaskId = 2, Hours = 4, Day = new DateTime(2019, 8, 3) });
                context.TaskNote.Add(new TaskNote() { TaskId = 2, Hours = 7, Day = new DateTime(2019, 8, 4) });
                context.TaskNote.Add(new TaskNote() { TaskId = 3, Hours = 8, Day = new DateTime(2019, 8, 1) });
                context.TaskNote.Add(new TaskNote() { TaskId = 3, Hours = 4, Day = new DateTime(2019, 8, 2) });
                context.TaskNote.Add(new TaskNote() { TaskId = 3, Hours = 7, Day = new DateTime(2019, 8, 3) });
                context.TaskNote.Add(new TaskNote() { TaskId = 4, Hours = 5, Day = new DateTime(2019, 7, 3) });
                context.TaskNote.Add(new TaskNote() { TaskId = 5, Hours = 8, Day = new DateTime(2019, 7, 3) });
                context.TaskNote.Add(new TaskNote() { TaskId = 6, Hours = 1, Day = new DateTime(2019, 7, 11) });                
                context.TaskNote.Add(new TaskNote() { TaskId = 8, Hours = 7, Day = new DateTime(2019, 6, 11) });
                context.TaskNote.Add(new TaskNote() { TaskId = 8, Hours = 4, Day = new DateTime(2019, 7, 11) });
                context.TaskNote.Add(new TaskNote() { TaskId = 7, Hours = 6, Day = new DateTime(2019, 7, 14) });

                context.TaskNote.Add(new TaskNote() { TaskId = 9, Hours = 10, Day = new DateTime(2019, 7, 1) });
                context.TaskNote.Add(new TaskNote() { TaskId = 10, Hours = 5, Day = new DateTime(2019, 7, 2) });
                context.TaskNote.Add(new TaskNote() { TaskId = 11, Hours = 20, Day = new DateTime(2019, 7, 3) });
                context.TaskNote.Add(new TaskNote() { TaskId = 12, Hours = 12, Day = new DateTime(2019, 7, 4) });
                context.TaskNote.Add(new TaskNote() { TaskId = 13, Hours = 80, Day = new DateTime(2019, 7, 5) });
                context.TaskNote.Add(new TaskNote() { TaskId = 14, Hours = 15, Day = new DateTime(2019, 7, 6) });
                context.TaskNote.Add(new TaskNote() { TaskId = 15, Hours = 40, Day = new DateTime(2019, 7, 7) });
                context.TaskNote.Add(new TaskNote() { TaskId = 16, Hours = 10, Day = new DateTime(2019, 7, 8) });
                context.TaskNote.Add(new TaskNote() { TaskId = 17, Hours = 22, Day = new DateTime(2019, 7, 9) });
                context.TaskNote.Add(new TaskNote() { TaskId = 18, Hours = 44, Day = new DateTime(2019, 7, 10) });
                context.TaskNote.Add(new TaskNote() { TaskId = 19, Hours = 1, Day = new DateTime(2019, 7, 14) });

                context.SaveChanges();
            }
        }
    }
}
