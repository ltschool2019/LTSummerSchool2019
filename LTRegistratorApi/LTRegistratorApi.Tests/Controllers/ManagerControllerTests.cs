using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Controllers;
using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace LTRegistratorApi.Tests.Controllers
{
    public class ManagerControllerTests : BaseControllerTests
    {
        private readonly ManagerController _managerController;

        public ManagerControllerTests()
        {
            _managerController = new ManagerController(Db, null);
        }

        [Theory]
        [InlineData(-1, HttpStatusCode.NotFound)] //Такого пользователя не существует
        [InlineData(1, HttpStatusCode.NotFound)] //Это не менеджер, а администратор
        [InlineData(3, HttpStatusCode.NotFound)] //Это не менеджер, а работник
        [InlineData(2, HttpStatusCode.OK)]
        public async void GetManagerProjectsTests(int employeeId, HttpStatusCode status)
        {
            var result = _managerController.GetManagerProjects(employeeId);
            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.OK)
                Assert.IsType<List<ProjectDto>>((result as ObjectResult).Value);
        }

        [Theory]
        [InlineData(2, -1, HttpStatusCode.NotFound)] //Такого пользователя не существует
        [InlineData(-1, 2, HttpStatusCode.NotFound)] //Такого проекта не существует
        [InlineData(2, 6, HttpStatusCode.NotFound)] //Пользователь уже на этом проекте
        [InlineData(1, 6, HttpStatusCode.OK)]
        public async void AssignProjectToEmployeeTests(int projectId, int employeeId, HttpStatusCode status)
        {
            var result = await _managerController.AssignProjectToEmployee(projectId, employeeId);
            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.OK)
                Assert.NotNull(Db.Set<ProjectEmployee>().SingleOrDefault(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId));
        }

        [Theory]
        [InlineData(2, -1, HttpStatusCode.NotFound)] //Такого пользователя не существует
        [InlineData(-1, 2, HttpStatusCode.NotFound)] //Такого проекта не существует
        [InlineData(3, 6, HttpStatusCode.NotFound)] //Пользователя нет на этом проекте
        [InlineData(2, 6, HttpStatusCode.OK)]
        public async void ReassignEmployeeFromProjectTests(int projectId, int employeeId, HttpStatusCode status)
        {
            var result = await _managerController.ReassignEmployeeFromProject(projectId, employeeId);
            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.OK)
                Assert.Null(Db.Set<ProjectEmployee>().FirstOrDefault(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId));
        }

        [Theory]
        [InlineData(1, -1, HttpStatusCode.NotFound)] //Такого пользователя не существует
        [InlineData(-1, 2, HttpStatusCode.NotFound)] //Такого проекта не существует
        [InlineData(3, 2, HttpStatusCode.OK)] //Найдутся пользователи с другого проекта, где этот менеджер не является менеджером
        [InlineData(2, 4, HttpStatusCode.OK)]
        public async void GetEmployeesTests(int projectId, int employeeId, HttpStatusCode status)
        {
            var result = await _managerController.GetEmployees(projectId, employeeId);
            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.OK)
                Assert.IsType<List<EmployeeDto>>((result as ObjectResult).Value);
        }
    }
}
