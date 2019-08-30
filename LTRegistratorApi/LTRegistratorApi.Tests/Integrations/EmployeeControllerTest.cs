using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LTRegistratorApi.Tests.Integrations
{
    public class EmployeeControllerTest : BaseControllerTest
    {

        public EmployeeControllerTest(CustomWebApplicationFactory<FakeStartup> factory) : base(factory)
        {
        }

        #region GetInfoAsync
        [Theory]
        // As manager
        [InlineData("b0b@yandex.ru", 2, HttpStatusCode.OK)]
        [InlineData("b0b@yandex.ru", -1, HttpStatusCode.NotFound)]
        // As employee
        [InlineData("eve.99@yandex.ru", 2, HttpStatusCode.Forbidden)]
        // As admin
        [InlineData("alice@mail.ru", 2, HttpStatusCode.OK)]
        public async void GetInfoAsync_GetWithAuthUser(string employeeEmail, int employeeId, HttpStatusCode statusCode)
        {
            var response = await GetResponse(employeeEmail, $"/api/employee/{employeeId}/info");

            Assert.True(response.StatusCode == statusCode);
            if (statusCode != HttpStatusCode.OK) return;

            response.EnsureSuccessStatusCode();
            var employee = await GetResponseContent<EmployeeDto>(response);

            Assert.True(employee != null);
        }
        #endregion

        #region GetLeavesAsync
        [Theory]
        // As manager
        [InlineData("b0b@yandex.ru", 2, HttpStatusCode.OK)]
        [InlineData("b0b@yandex.ru", 1, HttpStatusCode.OK)]
        [InlineData("b0b@yandex.ru", -1, HttpStatusCode.NotFound)]
        // As employee
        [InlineData("eve.99@yandex.ru", 2, HttpStatusCode.Forbidden)]
        // As admin
        [InlineData("alice@mail.ru", 2, HttpStatusCode.OK)]
        public async void GetLeavesAsync_GetWithAuthUser(string employeeEmail, int employeeId, HttpStatusCode statusCode)
        {
            var response = await GetResponse(employeeEmail, $"/api/employee/{employeeId}/leaves");

            Assert.True(response.StatusCode == statusCode);

            if (statusCode != HttpStatusCode.OK) return;

            response.EnsureSuccessStatusCode();
            var leaves = await GetResponseContent<ICollection<LeaveDto>>(response);
            using (var scope = Host.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var db = service.GetRequiredService<DbContext>();

                var entities = await db.Set<Leave>().Where(l => l.EmployeeId == employeeId).ToArrayAsync();

                Assert.True(leaves.Count == entities.Length);
            }
        }
        #endregion

        #region SetLeavesAsync
        [Fact]
        public async void SetLeavesAsync_SetWithAuthUser_ReturnsBadRequest()
        {
            var employeeId = 1;

            var response = await SendResponseWithBody(HttpMethod.Post, "b0b@yandex.ru", $"/api/employee/{employeeId}/leaves", null);

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("b0b@yandex.ru", 1, 6, 15, 6, 14, HttpStatusCode.BadRequest)] //Invalid leave. 
        [InlineData("b0b@yandex.ru", -1, 8, 6, 8, 8, HttpStatusCode.NotFound)] //User is not found.
        [InlineData("b0b@yandex.ru", 2, 8, 6, 8, 8, HttpStatusCode.BadRequest)] //Leave intersects with another.
        [InlineData("b0b@yandex.ru", 1, 8, 6, 8, 8, HttpStatusCode.OK)]
        public async void SetLeavesAsync_SetWithAuthUser(string emailEmployee, int employeeId, int startMonth, int startDay, int endMonth, int endDay, HttpStatusCode code)
        {
            var testItem = new LeaveInputDto
            {
                TypeLeave = TypeLeaveDto.Vacation,
                StartDate = new DateTime(2019, startMonth, startDay),
                EndDate = new DateTime(2019, endMonth, endDay)
            };

            var response = await SendResponseWithBody(HttpMethod.Post, emailEmployee, $"/api/employee/{employeeId}/leaves", new List<LeaveInputDto> { testItem }).ConfigureAwait(false);

            Assert.True(response.StatusCode == code);
            if (code == HttpStatusCode.OK)
            {
                using (var scope = Host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var db = services.GetRequiredService<DbContext>();

                    var result = await db.Set<Leave>().FirstOrDefaultAsync(l =>
                        l.StartDate == testItem.StartDate && l.EndDate == testItem.EndDate && l.EmployeeId == employeeId).ConfigureAwait(false);
                    Assert.NotNull(result);
                }
            }
        }
        #endregion

        #region UpdateLeavesAsync
        [Fact]
        public async void UpdateLeavesAsync_LeavesNotTransferred_ReturnsBadRequest()
        {
            var employeeId = 2;

            var response = await SendResponseWithBody(HttpMethod.Put, "b0b@yandex.ru", $"/api/employee/{employeeId}/leaves", null);

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Theory]
        // As manager
        [InlineData("b0b@yandex.ru", 2, 2, 11, 2, 14, HttpStatusCode.BadRequest)]
        [InlineData("b0b@yandex.ru", -1, 1, 2, 1, 4, HttpStatusCode.NotFound)] //User is not found.
        [InlineData("b0b@yandex.ru", 2, 1, 14, 1, 16, HttpStatusCode.OK)]
        // As employee
        [InlineData("eve.99@yandex.ru", 2, 1, 14, 1, 16, HttpStatusCode.Forbidden)]
        // As admin
        [InlineData("alice@mail.ru", 2, 1, 14, 1, 16, HttpStatusCode.OK)]
        public async void UpdateLeavesAsync_UpdateWithAuthUser(string employeeEmail, int employeeId, int startMonth, int startDay, int endMonth, int endDay, HttpStatusCode code)
        {
            var testItem = new LeaveDto
            {
                Id = 1,
                TypeLeave = TypeLeaveDto.Vacation,
                StartDate = new DateTime(2019, startMonth, startDay),
                EndDate = new DateTime(2019, endMonth, endDay)
            };

            var response = await SendResponseWithBody(HttpMethod.Put, employeeEmail, $"/api/employee/{employeeId}/leaves", new List<LeaveDto> { testItem }).ConfigureAwait(false);

            Assert.True(response.StatusCode == code);
            using (var scope = Host.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var db = service.GetRequiredService<DbContext>();
                var entity = await db.Set<Leave>().FirstOrDefaultAsync(l => l.StartDate == testItem.StartDate && l.EndDate == testItem.EndDate && l.EmployeeId == employeeId).ConfigureAwait(false);

                if (code == HttpStatusCode.OK)
                {
                    Assert.NotNull(entity);
                }
                else
                {
                    Assert.Null(entity);
                }
            }
        }
        #endregion

        [Theory]
        // As manager
        [InlineData("b0b@yandex.ru", 1, HttpStatusCode.BadRequest)] //Invalid leave (null). 
        [InlineData("b0b@yandex.ru", -1, HttpStatusCode.NotFound, 1)] //User is not found.
        [InlineData("b0b@yandex.ru", 2, HttpStatusCode.BadRequest, 1, 1)] //Leave intersects with myself.
        [InlineData("b0b@yandex.ru", 2, HttpStatusCode.BadRequest, -1)] //Leave is not found.
        [InlineData("b0b@yandex.ru", 2, HttpStatusCode.OK, 1, 2)]
        // As employee
        [InlineData("eve.99@yandex.ru", 2, HttpStatusCode.Forbidden, 1, 2)]
        // As admin
        [InlineData("alice@mail.ru", 2, HttpStatusCode.OK, 1, 2)]
        public async void DeleteLeavesAsync(string employeeEmail, int employeeId, HttpStatusCode code, params int[] leaveIds)
        {
            var url = leaveIds.Any()
                ? $"/api/employee/{employeeId}/leaves?{string.Join("&", leaveIds.Select(l => $"leaveId={l}"))}"
                : $"/api/employee/{employeeId}/leaves";
            var response = await SendResponseWithBody(HttpMethod.Delete, employeeEmail, url, null);

            Assert.True(response.StatusCode == code);

            if (code == HttpStatusCode.OK)
            {
                using (var scope = Host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var db = services.GetRequiredService<DbContext>();

                    var leaves = await db.Set<Leave>().Where(l => leaveIds.Contains(l.Id)).ToArrayAsync().ConfigureAwait(false);


                    Assert.False(leaves.Any());
                }
            }
        }
    }
}
