using LTRegistrator.BLL.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LTRegistratorApi.Tests
{
    public static class Initializer
    {
        public static void Initialize(DbContext db)
        {
            db.Database.EnsureCreated();
            if (db.Set<Employee>().Any() || db.Set<Project>().Any() || db.Set<ProjectEmployee>().Any() ||
                db.Set<Task>().Any() || db.Set<TaskNote>().Any()) return;

            var leaveBob = new Leave[]
            {
                new Leave() { Id = 1, StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 2, StartDate = new DateTime(2019, 8, 6), EndDate = new DateTime(2019, 8, 8), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 3, StartDate = new DateTime(2019, 8, 1), EndDate = new DateTime(2019, 8, 4), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 4, StartDate = new DateTime(2019, 2, 10), EndDate = new DateTime(2019, 2, 13), TypeLeave = TypeLeave.SickLeave }
            };
            var leaveEve = new Leave[]
            {
                new Leave() { Id= 5, StartDate = new DateTime(2019, 2, 10), EndDate = new DateTime(2019, 3, 1), TypeLeave = TypeLeave.Training }
            };
            var leaveCarol = new Leave[]
            {
                new Leave() { Id = 6, StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 7, StartDate = new DateTime(2019, 2, 1), EndDate = new DateTime(2019, 2, 15), TypeLeave = TypeLeave.Training },
                new Leave() { Id = 8, StartDate = new DateTime(2019, 3, 1), EndDate = new DateTime(2019, 4, 1), TypeLeave = TypeLeave.SickLeave }
            };
            var leaveFrank = new Leave[]
            {
                new Leave() { Id = 9, StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 10, StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Idle }
            };

            var employees = new List<Employee>()
            {
                new Employee() {Id=1, FirstName = "Alice", SecondName = "Brown", Mail = "alice@mail.ru", Rate = 1.5, Leaves = new List<Leave>()},
                new Employee() {Id=2, FirstName = "Bob", SecondName = "Johnson", Mail = "b0b@yandex.ru", Leaves = leaveBob, Rate = 1 },
                new Employee() {Id=3, FirstName = "Eve", SecondName = "Williams", Mail = "eve.99@yandex.ru", Leaves = leaveEve, Rate = 1.25, ManagerId = 2 },
                new Employee() {Id=4, FirstName = "Carol", SecondName = "Smith", Mail = "car0l@mail.ru", Leaves = leaveCarol, Rate = 1 },
                new Employee() {Id=5, FirstName = "Dave", SecondName = "Jones", Mail = "dave.99@mail.ru", Rate = 1, ManagerId = 2 ,Leaves = new List<Leave>()},
                new Employee() {Id=6, FirstName = "Frank", SecondName = "Florence", Mail = "frank.99@mail.ru", Leaves = leaveFrank, Rate = 0.25, ManagerId = 4 }
            };

            {
                var dbSet = db.Set<Employee>();
                foreach (var employee in employees)
                {
                    dbSet.Add(employee);
                }
                db.SaveChanges();
            }

            {
                var dbSet = db.Set<Project>();
                dbSet.Add(new Project() { Name = "FOSS" });
                dbSet.Add(new Project() { Name = "EMIAS" });
                dbSet.Add(new Project() { Name = "Area 9" });
                db.SaveChanges();
            }

            {
                var dbSet = db.Set<ProjectEmployee>();
                dbSet.Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 1 });
                dbSet.Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 2 });
                dbSet.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 2 });
                dbSet.Add(new ProjectEmployee() { ProjectId = 1, EmployeeId = 3 });
                dbSet.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 3 });
                dbSet.Add(new ProjectEmployee() { ProjectId = 3, EmployeeId = 3 });
                dbSet.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 4 });
                dbSet.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 5 });
                dbSet.Add(new ProjectEmployee() { ProjectId = 2, EmployeeId = 6 });
                db.SaveChanges();
            }

            {
                var dbSet = db.Set<Task>();
                dbSet.Add(new Task() { ProjectId = 1, EmployeeId = 1, Name = "FOSS" });
                dbSet.Add(new Task() { ProjectId = 1, EmployeeId = 2, Name = "FOSS" });
                dbSet.Add(new Task() { ProjectId = 2, EmployeeId = 2, Name = "EMIAS" });
                dbSet.Add(new Task() { ProjectId = 1, EmployeeId = 3, Name = "FOSS" });
                dbSet.Add(new Task() { ProjectId = 2, EmployeeId = 3, Name = "EMIAS" });
                dbSet.Add(new Task() { ProjectId = 3, EmployeeId = 3, Name = "Area 9" });
                dbSet.Add(new Task() { ProjectId = 2, EmployeeId = 4, Name = "EMIAS" });
                dbSet.Add(new Task() { ProjectId = 2, EmployeeId = 5, Name = "EMIAS" });
                db.SaveChanges();
            }

            {
                var dbSet = db.Set<TaskNote>();
                dbSet.Add(new TaskNote() { TaskId = 1, Hours = 4, Day = new DateTime(2019, 1, 1) });
                dbSet.Add(new TaskNote() { TaskId = 2, Hours = 7, Day = new DateTime(2019, 8, 2) });
                dbSet.Add(new TaskNote() { TaskId = 2, Hours = 4, Day = new DateTime(2019, 8, 3) });
                dbSet.Add(new TaskNote() { TaskId = 2, Hours = 7, Day = new DateTime(2019, 8, 4) });
                dbSet.Add(new TaskNote() { TaskId = 3, Hours = 8, Day = new DateTime(2019, 8, 1) });
                dbSet.Add(new TaskNote() { TaskId = 3, Hours = 4, Day = new DateTime(2019, 8, 2) });
                dbSet.Add(new TaskNote() { TaskId = 3, Hours = 7, Day = new DateTime(2019, 8, 3) });
                dbSet.Add(new TaskNote() { TaskId = 4, Hours = 5, Day = new DateTime(2019, 7, 3) });
                dbSet.Add(new TaskNote() { TaskId = 5, Hours = 8, Day = new DateTime(2019, 7, 3) });
                dbSet.Add(new TaskNote() { TaskId = 6, Hours = 1, Day = new DateTime(2019, 7, 11) });
                dbSet.Add(new TaskNote() { TaskId = 8, Hours = 7, Day = new DateTime(2019, 6, 11) });
                dbSet.Add(new TaskNote() { TaskId = 8, Hours = 4, Day = new DateTime(2019, 7, 11) });
                dbSet.Add(new TaskNote() { TaskId = 7, Hours = 6, Day = new DateTime(2019, 7, 14) });
                db.SaveChanges();
            }
        }
    }
}
