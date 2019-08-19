using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Services.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LTRegistrator.BLL.Services.Tests.Services
{
    public class EmployeeServiceTests
    {
        LTRegistratorDbContext _dbContext;
        EmployeeService _employeeService;
        public EmployeeServiceTests()
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=productsdb;Trusted_Connection=True;";
            var options = new DbContextOptionsBuilder<LTRegistratorDbContext>()
                .UseInMemoryDatabase(connectionString).Options;
            _dbContext = new LTRegistratorDbContext(options);
            //_employeeService = new EmployeeService() DO
        }

        [Fact]
        public async void GetByIdAsync_UserNotFound_ReturnsBadRequest()
        {
            var userId = 1;
            _dbContext.Set<Employee>().Add(new Employee
            {
                Id = 1,
                FirstName = "Anton",
                SecondName = "Sapa",
                Mail = "alice@mail.ru",
                MaxRole = RoleType.Administrator,
                Rate = 1.5,
                Leaves = new List<Leave>()
            });
            await _dbContext.SaveChangesAsync();

            var result = await _employeeService.GetByIdAsync(userId);

            Assert.IsType<Response<Employee>>(result);
            Assert.NotNull(result.Result);
            Assert.Null(result.Error);
        }
    }
}
