using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

            #region Add Employees
            if (!context.Set<Employee>().Any())
            {
                context.Set<Employee>().Add(new Employee() { FirstName = "Александр", SecondName = "Москвин", Mail = "moskvin@mail.ru", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 10});
                context.Set<Employee>().Add(new Employee() { FirstName = "Ольга", SecondName = "Калатуша", Mail = "kalatusha@mail.ru", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 10});
                context.Set<Employee>().Add(new Employee() { FirstName = "Антон", SecondName = "Сапановский", Mail = "sapik@mail.ru", MaxRole = RoleType.Administrator, Rate = 1, ManagerId = 10, Leaves = new List<Leave>
                {
                    new Leave() {StartDate = new DateTime(2019, 8, 22), EndDate = new DateTime (2019, 9,9), TypeLeave = TypeLeave.Vacation}
                }});
                context.Set<Employee>().Add(new Employee() { FirstName = "Анна", SecondName = "Степакова", Mail = "stepakova@mail.ru", MaxRole = RoleType.Employee, Rate = 0.5, ManagerId = 10, Leaves = new List<Leave>
                {
                    new Leave() {StartDate = new DateTime(2019, 9, 5), EndDate = new DateTime(2019,9, 9), TypeLeave = TypeLeave.Training},
                    new Leave() { StartDate = new DateTime(2019, 9, 11), EndDate = new DateTime(2019, 9,14), TypeLeave = TypeLeave.Idle}
                }});
                context.Set<Employee>().Add(new Employee() { FirstName = "Дмитрий", SecondName = "Павлов", Mail = "pavlov@mail.ru", MaxRole = RoleType.Employee, Rate = 0.75, ManagerId = 10 , Leaves = new List<Leave>
                {
                    new Leave() {StartDate =  new DateTime(2019, 9, 3), EndDate = new DateTime(2019, 9, 11), TypeLeave = TypeLeave.Idle}
                }});
                context.Set<Employee>().Add(new Employee() { FirstName = "Ольга", SecondName = "Гуляева", Mail = "guliaeva@mail.ru", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 10 });
                context.Set<Employee>().Add(new Employee() { FirstName = "Павел", SecondName = "Костин", Mail = "kostin@mail.ru", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 10 });
                context.Set<Employee>().Add(new Employee() { FirstName = "Михаил", SecondName = "Ворожба", Mail = "vorojba@mail.ru", MaxRole = RoleType.Employee, Rate = 1.25, ManagerId = 10 });
                context.Set<Employee>().Add(new Employee() { FirstName = "Юлия", SecondName = "Ильиных", Mail = "ilinykh@mail.ru", MaxRole = RoleType.Employee, Rate = 0.5, ManagerId = 10 });

                context.Set<Employee>().Add(new Employee { FirstName = "Татьяна", SecondName = "Елисеева", Mail = "eliseeva@mail.ru", MaxRole = RoleType.Manager, Rate = 1.25, ManagerId = null });

                context.SaveChanges();

                foreach (var employee in context.Set<Employee>())
                {
                    var user = new User
                    {
                        UserName = employee.Mail,
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
            #endregion

            #region Add Projects
            if (!context.Set<Project>().Any())
            {
                context.Set<Project>().Add(new Project() { Name = "LTRegistrator Frontend" });
                context.Set<Project>().Add(new Project() { Name = "LTRegistrator Backend" });
                context.SaveChanges();
            }
            #endregion

            #region Add ProjectEmployees
            if (!context.Set<ProjectEmployee>().Any())
            {
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 10, Role = RoleType.Manager });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 10, Role = RoleType.Manager });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 2, Role = RoleType.Employee });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 4, Role = RoleType.Employee });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 5, Role = RoleType.Employee });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 6, Role = RoleType.Employee });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 1, Role = RoleType.Employee });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 3, Role = RoleType.Employee });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 3, Role = RoleType.Employee });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 7, Role = RoleType.Employee });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 8, Role = RoleType.Employee });
                context.Set<ProjectEmployee>().Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 9, Role = RoleType.Employee });

                context.SaveChanges();
            }
            #endregion

            #region Add Tasks
            if (!context.Set<Task>().Any())
            {
                context.Set<Task>().Add(new Task() { ProjectId = 2, EmployeeId = 1, Name = "LTRegistrator Backend" });
                context.Set<Task>().Add(new Task() { ProjectId = 1, EmployeeId = 2, Name = "LTRegistrator Frontend" });
                context.Set<Task>().Add(new Task() { ProjectId = 2, EmployeeId = 3, Name = "LTRegistrator Backend" });
                context.Set<Task>().Add(new Task() { ProjectId = 1, EmployeeId = 3, Name = "LTRegistrator Frontend" });
                context.Set<Task>().Add(new Task() { ProjectId = 1, EmployeeId = 4, Name = "LTRegistrator Frontend" });
                context.Set<Task>().Add(new Task() { ProjectId = 1, EmployeeId = 5, Name = "LTRegistrator Frontend" });
                context.Set<Task>().Add(new Task() { ProjectId = 1, EmployeeId = 6, Name = "LTRegistrator Frontend" });
                context.Set<Task>().Add(new Task() { ProjectId = 2, EmployeeId = 7, Name = "LTRegistrator Backend" });
                context.Set<Task>().Add(new Task() { ProjectId = 2, EmployeeId = 8, Name = "LTRegistrator Backend" });
                context.Set<Task>().Add(new Task() { ProjectId = 2, EmployeeId = 9, Name = "LTRegistrator Backend" });
                context.SaveChanges();
            }
            #endregion

            #region Add TaskNotes
            if (!context.Set<TaskNote>().Any())
            {
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 1, Hours = 4, Day = new DateTime(2019, 9, 1) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 1, Hours = 2, Day = new DateTime(2019, 9, 2) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 1, Hours = 8, Day = new DateTime(2019, 9, 4) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 1, Hours = 8, Day = new DateTime(2019, 9, 6) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 1, Hours = 8, Day = new DateTime(2019, 9, 12) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 1, Hours = 10, Day = new DateTime(2019, 9, 14) });
                
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 10, Day = new DateTime(2019, 9, 2) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 8, Day = new DateTime(2019, 9, 4) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 4, Day = new DateTime(2019, 9, 6) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 6, Day = new DateTime(2019, 9, 7) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 5, Day = new DateTime(2019, 9, 10) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 11, Day = new DateTime(2019, 9, 13) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 2, Hours = 2, Day = new DateTime(2019, 9, 14) });

                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 3, Hours = 5, Day = new DateTime(2019, 9, 11) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 3, Hours = 1, Day = new DateTime(2019, 9, 12) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 3, Hours = 10, Day = new DateTime(2019, 9, 13) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 3, Hours = 12, Day = new DateTime(2019, 9, 10) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 4, Hours = 12, Day = new DateTime(2019, 9, 12) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 4, Hours = 12, Day = new DateTime(2019, 9, 13) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 4, Hours = 12, Day = new DateTime(2019, 9, 14) });

                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 5, Hours = 6, Day = new DateTime(2019, 9, 2) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 5, Hours = 8, Day = new DateTime(2019, 9, 3) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 5, Hours = 7, Day = new DateTime(2019, 9, 4) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 5, Hours = 13, Day = new DateTime(2019, 9, 10) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 5, Hours = 4, Day = new DateTime(2019, 9, 15) });

                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 6, Hours = 10, Day = new DateTime(2019, 9, 1) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 6, Hours = 12, Day = new DateTime(2019, 9, 2) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 6, Hours = 14, Day = new DateTime(2019, 9, 12) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 6, Hours = 18, Day = new DateTime(2019, 9, 13) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 6, Hours = 10, Day = new DateTime(2019, 9, 14) });

                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 8, Day = new DateTime(2019, 9, 1) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 8, Day = new DateTime(2019, 9, 2) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 8, Day = new DateTime(2019, 9, 3) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 8, Day = new DateTime(2019, 9, 4) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 8, Day = new DateTime(2019, 9, 5) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 8, Day = new DateTime(2019, 9, 6) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 8, Day = new DateTime(2019, 9, 7) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 8, Day = new DateTime(2019, 9, 8) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 7, Hours = 8, Day = new DateTime(2019, 9, 9) });

                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 8, Hours = 2, Day = new DateTime(2019, 9, 1) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 8, Hours = 4, Day = new DateTime(2019, 9, 4) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 8, Hours = 6, Day = new DateTime(2019, 9, 5) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 8, Hours = 4, Day = new DateTime(2019, 9, 7) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 8, Hours = 3, Day = new DateTime(2019, 9, 9) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 8, Hours = 12, Day = new DateTime(2019, 9, 10) });

                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 9, Hours = 12, Day = new DateTime(2019, 9, 5) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 9, Hours = 6, Day = new DateTime(2019, 9, 7) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 9, Hours = 7, Day = new DateTime(2019, 9, 8) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 9, Hours = 1, Day = new DateTime(2019, 9, 9) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 9, Hours = 12, Day = new DateTime(2019, 9, 10) });
                    
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 10, Hours = 8, Day = new DateTime(2019, 9, 1) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 10, Hours = 8, Day = new DateTime(2019, 9, 2) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 10, Hours = 8, Day = new DateTime(2019, 9, 3) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 10, Hours = 8, Day = new DateTime(2019, 9, 4) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 10, Hours = 6, Day = new DateTime(2019, 9, 5) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 10, Hours = 6, Day = new DateTime(2019, 9, 6) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 10, Hours = 6, Day = new DateTime(2019, 9, 7) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 10, Hours = 2, Day = new DateTime(2019, 9, 8) });
                context.Set<TaskNote>().Add(new TaskNote() { TaskId = 10, Hours = 1, Day = new DateTime(2019, 9, 9) });

                context.SaveChanges();
            }
            #endregion
        }
    }
}
