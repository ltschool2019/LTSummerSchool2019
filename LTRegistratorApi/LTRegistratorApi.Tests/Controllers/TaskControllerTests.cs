using LTRegistratorApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Model;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace LTRegistratorApi.Tests.Controllers
{
    public class TaskControllerTests : BaseControllerTests
    {
        private readonly TaskController _taskController;

        public TaskControllerTests()
        {
            _taskController = new TaskController(Db);
        }

        [Theory]
        [InlineData(1, 2, true, HttpStatusCode.BadRequest)] //Нельзя добавить второй проект с таким же именем
        [InlineData(3, 2, true, HttpStatusCode.BadRequest)] //Нельзя добавить Task для Employee, который не работает на этом проекте (ProjectEmployee)
        [InlineData(2, -1, true, HttpStatusCode.BadRequest)] //Такого пользователя не существует
        [InlineData(-1, 2, true, HttpStatusCode.NotFound)] //Такого проекта не существует
        [InlineData(2, 6, false, HttpStatusCode.BadRequest)] //Нельзя в List<TaskNote> передать null
        [InlineData(2, 6, true, HttpStatusCode.OK)]
        public async void AddTaskTests(int projectId, int employeeId, bool correctTaskNotes, HttpStatusCode status)
        {
            var task = new TaskInputDto() { Name = "EMIAS", TaskNotes = null };
            if (correctTaskNotes)
                task.TaskNotes = new List<TaskNoteDto>
                {
                    new TaskNoteDto
                    {
                        Hours = 5,
                        Day = new DateTime(2019, 7, 7)
                    }
                };
            var result = await _taskController.AddTask(projectId, employeeId, task);
            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.OK) //Проверяем, что добавлено 
                Assert.NotNull(Db.Set<Task>().FirstOrDefault(t => t.ProjectId == projectId && t.EmployeeId == employeeId));
        }

        [Theory]
        [InlineData(2, -1, HttpStatusCode.NotFound)] //Такого пользователя не существует
        [InlineData(-1, 2, HttpStatusCode.NotFound)] //Такого проекта не существует
        [InlineData(2, 6, HttpStatusCode.NotFound)] //Пользователь с таким проектом ещё не делал ничего
        [InlineData(1, 6, HttpStatusCode.NotFound)] //Пользователя нет на этом проекте
        [InlineData(1, 1, HttpStatusCode.OK)] 
        public async void GetTasksTests(int projectId, int employeeId, HttpStatusCode status)
        {
            var result = await _taskController.GetTasks(projectId, employeeId, new DateTime(2019, 1, 1), new DateTime(2019, 12, 30));
            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.OK)
                Assert.IsType<List<TaskDto>>((result as ObjectResult).Value);
        }

        [Theory]
        [InlineData(-1, "EMIAS", 2, HttpStatusCode.NotFound)] //Такой Task не существует
        //[InlineData(8, "EMIAS", -1, HttpStatusCode.NotFound)] //Такого пользователя не существует
        [InlineData(8, "HELP", 2, HttpStatusCode.NotFound)] //Такого проекта не существует
        [InlineData(8, "Area 9", 3, HttpStatusCode.NotFound)] //Пользователя нет на этом проекте
        [InlineData(8, "EMIAS", 2, HttpStatusCode.OK)]
        public async void UpdateTaskTests(int taskId, string nameTask, int employeeId, HttpStatusCode status)
        {
            var result = await _taskController.UpdateTask(
                new TaskInputDto()
                {
                    Id = taskId,
                    Name = nameTask,
                    TaskNotes = new List<TaskNoteDto>
                    {
                        new TaskNoteDto
                        {
                            Hours = 5,
                            Day = new DateTime(2019, 7, 7)
                        }
                    }
                }, employeeId);
            Assert.Equal((int)status, ToHttpStatusCodeResult(result));

            if(status == HttpStatusCode.OK)
            {
                var task = Db.Set<Task>().SingleOrDefault(t => t.Id == taskId);
                Assert.NotNull(task);
                Assert.Equal(nameTask, task.Name);
            }
        }

        [Theory]
        [InlineData(-1, 2, HttpStatusCode.NotFound)] //Такой Task не существует
        //[InlineData(8, -1, HttpStatusCode.NotFound)] //Такого пользователя не существует
        [InlineData(8, 2, HttpStatusCode.OK)]
        public async void DeleteTaskTests(int taskId, int employeeId, HttpStatusCode status)
        {
            var result = await _taskController.DeleteTask(taskId, employeeId);
            Assert.Equal((int)status, ToHttpStatusCodeResult(result));

            if (status == HttpStatusCode.OK)
            {
                Assert.Null(Db.Set<Task>().FirstOrDefault(t => t.Id == taskId));
            }
        }
    }
}
