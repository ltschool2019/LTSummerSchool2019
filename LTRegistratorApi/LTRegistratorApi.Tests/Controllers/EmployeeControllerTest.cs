using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistratorApi.Controllers;
using LTRegistratorApi.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistratorApi.Mappings;
using LTRegistratorApi.Model;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LTRegistratorApi.Tests.Controllers
{
    public class EmployeeControllerTest
    {
        #region Private Variables
        private readonly EmployeeController _employeeController;
        private readonly IEmployeeService _employeeService;
        #endregion

        #region Constructor
        public EmployeeControllerTest()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile<DataMappingProfileWeb>();
            });
            var mapper = mappingConfig.CreateMapper();
            _employeeService = new EmployeeServiceFake(mapper);
            _employeeController = new EmployeeController(_employeeService, mapper);
        }
        #endregion

        #region GetInfoByIdAsync
        [Fact]
        public async void GetInfoById_UserIsFounded_ReturnsOkResult()
        {
            var userId = 2;
            var result = await _employeeController.GetInfoAsync(userId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<EmployeeDto>(result.ToObjectResult().Value);
        }

        [Fact]
        public async void GetInfoById_UserNotFound_ReturnsNotFoundResult()
        {
            var userId = -1;
            var result = await _employeeController.GetInfoAsync(userId);
            var objectResult = result.ToObjectResult();

            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
            Assert.False(string.IsNullOrWhiteSpace((string)objectResult.Value));
        }
        #endregion

        #region GetLeavesAsync
        [Fact]
        public async void GetLeavesAsync_UserContainsLeaves_ReturnsOkResult()
        {
            var userId = 2;
            var result = await _employeeController.GetLeavesAsync(userId);
            var objectResult = result.ToObjectResult();

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<LeaveDto>>(objectResult.Value);
        }

        [Fact]
        public async void GetLeavesAsync_UserNotFound_ReturnsNotFound()
        {
            var userId = -1;
            var result = await _employeeController.GetLeavesAsync(userId);
            var objectResult = result.ToObjectResult();

            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
            Assert.IsType<string>(objectResult.Value);
            Assert.False(string.IsNullOrWhiteSpace(objectResult.Value as string));
        }
        #endregion

        #region SetLeavesAsync
        [Fact]
        public async void SetLeavesAsync_LeavesNotTransferred_ReturnsBadRequest()
        {
            var userId = 1;

            var result = await _employeeController.SetLeavesAsync(userId, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void SetLeavesAsync_ModelStateInvalid_ReturnsBadRequest()
        {
            var testItems = new List<LeaveInputDto>()
            {
                new LeaveInputDto()
                {
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, 6, 15),
                    EndDate = new DateTime(2019, 6, 14),
                }
            };
            var userId = 1;

            var result = await _employeeController.SetLeavesAsync(userId, testItems);
            var objectResult = result.ToObjectResult();

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ModelStateDictionary>(objectResult.Value);
        }

        [Fact]
        public async void SetLeaves_UserNotFound_ReturnsBadRequest()
        {
            var testItems = new List<LeaveInputDto>()
            {
                new LeaveInputDto()
                {
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, 6, 15),
                    EndDate = new DateTime(2019, 6, 16),
                }
            };
            var userId = -1;

            var result = await _employeeController.SetLeavesAsync(userId, testItems);
            var objectResult = result.ToObjectResult();

            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async void SetLeaves_CanNotMergeLeaves_ReturnsBadRequest()
        {
            var testItems = new List<LeaveInputDto>()
            {
                new LeaveInputDto()
                {
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, 8, 6),
                    EndDate = new DateTime(2019, 8, 8),
                }
            };
            var userId = 2;

            var result = await _employeeController.SetLeavesAsync(userId, testItems);
            var objectResult = result.ToObjectResult();

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async void SetLeaves_LeaveCorrect_ReturnsOkRequest()
        {
            var testItems = new List<LeaveInputDto>()
            {
                new LeaveInputDto()
                {
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, 8, 6),
                    EndDate = new DateTime(2019, 8, 8),
                }
            };
            var userId = 1;

            var result = await _employeeController.SetLeavesAsync(userId, testItems);

            Assert.IsType<OkResult>(result);

            var serviceResult = await _employeeService.GetByIdAsync(userId);
            var testItem = testItems.FirstOrDefault();
            var leave = serviceResult.Result.Leaves.FirstOrDefault(l =>
                l.StartDate == testItem.StartDate && l.EndDate == testItem.EndDate);

            Assert.NotNull(leave);
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

        [Fact]
        public async void UpdateLeaveAsync_UserNotFound_ReturnsNotFoundRequest()
        {
            var userId = -1;
            var testItems = new List<LeaveDto>()
            {
                new LeaveDto()
                {
                    Id = 11,
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, 8, 6),
                    EndDate = new DateTime(2019, 8, 8),
                }
            };

            var result = await _employeeController.UpdateLeavesAsync(userId, testItems);
            var objectResult = result.ToObjectResult();

            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async void UpdateLeaveAsync_CanNotMergeLeaves_ReturnsNotFoundRequest()
        {
            var userId = 2;
            var testItems = new List<LeaveDto>()
            {
                new LeaveDto()
                {
                    Id = 1,
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, 8, 6),
                    EndDate = new DateTime(2019, 8, 8),
                },
                new LeaveDto()
                {
                    Id = 2,
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, 8, 7),
                    EndDate = new DateTime(2019, 8, 10),
                }
            };

            var result = await _employeeController.UpdateLeavesAsync(userId, testItems);
            var objectResult = result.ToObjectResult();

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);

            //Проверям, не изменились ли отпуска
            var user = await _employeeService.GetByIdAsync(userId);
            var leave = user.Result.Leaves.FirstOrDefault(l => l.Id == testItems.First().Id);
            Assert.True(leave.StartDate != testItems.First().StartDate 
                        && leave.EndDate != testItems.First().EndDate);

            leave = user.Result.Leaves.FirstOrDefault(l => l.Id == testItems.Last().Id);
            Assert.True(leave.StartDate != testItems.Last().StartDate
                        && leave.EndDate != testItems.Last().EndDate);
        }

        [Fact]
        public async void UpdateLeaveAsync_LeavesCorrect_ReturnsNotFoundRequest()
        {
            var userId = 2;
            var testItems = new List<LeaveDto>()
            {
                new LeaveDto()
                {
                    Id = 1,
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, 7, 1),
                    EndDate = new DateTime(2019, 7, 2),
                },
                new LeaveDto()
                {
                    Id = 2,
                    TypeLeave = TypeLeaveDto.Vacation,
                    StartDate = new DateTime(2019, 7, 4),
                    EndDate = new DateTime(2019, 7, 5),
                }
            };

            var result = await _employeeController.UpdateLeavesAsync(userId, testItems);

            Assert.IsType<OkResult>(result);

            //Проверяем, дейсвтительно ли были изменены отпуска
            var user = await _employeeService.GetByIdAsync(userId);
            var leave = user.Result.Leaves.FirstOrDefault(l => l.Id == testItems.First().Id);
            Assert.True(leave.StartDate == testItems.First().StartDate
                        && leave.EndDate == testItems.First().EndDate);

            leave = user.Result.Leaves.FirstOrDefault(l => l.Id == testItems.Last().Id);
            Assert.True(leave.StartDate == testItems.Last().StartDate
                        && leave.EndDate == testItems.Last().EndDate);
        }
        #endregion
    }

    public static class Extensions
    {
        public static ObjectResult ToObjectResult(this ActionResult actionResult)
        {
            var objectResult = actionResult as ObjectResult;
            if (objectResult == null) throw new ArgumentException();

            return objectResult;
        }
    }
}
