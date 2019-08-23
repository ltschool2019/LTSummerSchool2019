using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistratorApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LTRegistratorApi.Model;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using LTRegistrator.BLL.Services.Services;
using LTRegistrator.Domain.Entities;

namespace LTRegistratorApi.Tests.Controllers
{
    public class EmployeeControllerTests : BaseControllerTests
    {
        private readonly EmployeeController _employeeController;
        private readonly IEmployeeService _employeeService;

        public EmployeeControllerTests()
        {
            _employeeService = new EmployeeService(Db, Mapper);
            _employeeController = new EmployeeController(_employeeService, Mapper, Db);
        }

        [Theory]
        [InlineData(2, HttpStatusCode.OK)]
        [InlineData(-1, HttpStatusCode.NotFound)] //User is not found.
        public async void GetInfoByIdAsync_Acess(int userId, HttpStatusCode status)
        {
            var result = await _employeeController.GetInfoAsync(userId);
            var objResult = result as ObjectResult;

            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.OK)
                Assert.IsType<EmployeeDto>(objResult.Value);
            else
                Assert.False(string.IsNullOrWhiteSpace((string)objResult.Value));
        }

        [Theory]
        [InlineData(2, HttpStatusCode.OK)]
        [InlineData(-1, HttpStatusCode.NotFound)] //User is not found.
        public async void GetLeavesAsync_Acess(int userId, HttpStatusCode status)
        {
            var result = await _employeeController.GetLeavesAsync(userId);
            var objResult = result as ObjectResult;

            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.OK)
            {
                Assert.IsType<List<LeaveDto>>(objResult.Value);
            }
            else
            {
                Assert.IsType<string>(objResult.Value);
                Assert.False(string.IsNullOrWhiteSpace(objResult.Value as string));
            }
        }

        #region SetLeavesAsync
        [Fact]
        public async void SetLeavesAsync_LeavesNotTransferred_ReturnsBadRequest()
        {
            var userId = 1;

            var result = await _employeeController.SetLeavesAsync(userId, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1, 6, 15, 6, 14, HttpStatusCode.BadRequest)] //Invalid leave. 
        [InlineData(-1, 6, 15, 6, 14, HttpStatusCode.NotFound)] //User is not found.
        [InlineData(2, 8, 6, 8, 8, HttpStatusCode.BadRequest)] //Leave intersects with another.
        [InlineData(1, 8, 6, 8, 8, HttpStatusCode.OK)]
        public async void SetLeavesAsyncTests(int userId, int startMonth, int startDay, int endMonth, int endDay, HttpStatusCode status)
        {
            var testItems = new List<LeaveInputDto>()
            {
                new LeaveInputDto()
                {
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, startMonth, startDay),
                    EndDate = new DateTime(2019, endMonth, endDay)
                }
            };

            var result = await _employeeController.SetLeavesAsync(userId, testItems);
            
            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.OK)
            {
                var testItem = testItems.FirstOrDefault();
                var leave = Db.Set<Leave>().FirstOrDefault(l =>
                    l.StartDate == testItem.StartDate && l.EndDate == testItem.EndDate);

                Assert.NotNull(leave);
            }
        }
        #endregion

        #region UpdateLeavesAsync
        [Fact]
        public async void UpdateLeaveAsync_LeavesNotTransferred_ReturnsBadRequest()
        {
            var userId = 1;
            var result = await _employeeController.UpdateLeavesAsync(userId, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(2, 8, 6, 8, 8, 8, 7, 8, 10, HttpStatusCode.BadRequest, false)] //Leaves intersect.
        [InlineData(-1, 7, 1, 7, 2, 7, 4, 7, 5, HttpStatusCode.NotFound, false)] //User is not found.
        [InlineData(2, 7, 1, 7, 2, 7, 4, 7, 5, HttpStatusCode.OK, true)]

        public async void UpdateLeaveAsyncTests(int userId, int startMonth1, int startDay1, int endMonth1, int endDay1,
            int startMonth2, int startDay2, int endMonth2, int endDay2, HttpStatusCode status, bool haveChanged)
        {
            var testItems = new List<LeaveDto>()
            {
                new LeaveDto()
                {
                    Id = 1,
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, startMonth1, startDay1),
                    EndDate = new DateTime(2019, endMonth1, endDay1)
                },
                new LeaveDto()
                {
                    Id = 2,
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, startMonth2, startDay2),
                    EndDate = new DateTime(2019, endMonth2, endDay2)
                }
            };

            var result = await _employeeController.UpdateLeavesAsync(userId, testItems);
            var objectResult = result as ObjectResult;

            Assert.Equal((int)status, ToHttpStatusCodeResult(result));
            if (status == HttpStatusCode.NotFound) //Не умеем проверять для несуществующего пользователя изменения
                return;

            var user = await _employeeService.GetByIdAsync(userId);
            var leave = user.Result.Leaves.FirstOrDefault(l => l.Id == testItems.First().Id);
            if (haveChanged) //We check that the leaves have changed.
            {
                Assert.True(leave.StartDate == testItems.First().StartDate
                            && leave.EndDate == testItems.First().EndDate);

                leave = user.Result.Leaves.FirstOrDefault(l => l.Id == testItems.Last().Id);
                Assert.True(leave.StartDate == testItems.Last().StartDate
                            && leave.EndDate == testItems.Last().EndDate);
            }
            else //We check that the leaves have not changed.
            {
                Assert.True(leave.StartDate != testItems.First().StartDate
                            && leave.EndDate != testItems.First().EndDate);

                leave = user.Result.Leaves.FirstOrDefault(l => l.Id == testItems.Last().Id);
                Assert.True(leave.StartDate != testItems.Last().StartDate
                            && leave.EndDate != testItems.Last().EndDate);
            }
        }
        #endregion
    }
}