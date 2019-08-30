using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LTRegistratorApi.Model;
using Newtonsoft.Json;
using Xunit;

namespace LTRegistratorApi.Tests.Integrations
{
    public class EmployeeControllerTest : IClassFixture<CustomWebApplicationFactory<FakeStartup>>
    {
        private readonly HttpClient _client;

        public EmployeeControllerTest(CustomWebApplicationFactory<FakeStartup> factory)
        {
            _client = factory.CreateClient(factory.ClientOptions);
        }

        [Theory]
        // As manager
        [InlineData("b0b@yandex.ru", 2, HttpStatusCode.OK)]
        [InlineData("b0b@yandex.ru" , - 1, HttpStatusCode.NotFound)]
        // As employee
        [InlineData("eve.99@yandex.ru", 2, HttpStatusCode.Forbidden)]
        public async void GetInfoAsync_GetWithAuthUser_ReturnsEmployeeDto(string employeeEmail, int employeeId, HttpStatusCode statusCode)
        {
            var response = await GetResponse(employeeEmail, $"/api/employee/{employeeId}/info");

            Assert.True(response.StatusCode == statusCode);
            if (statusCode != HttpStatusCode.OK) return;

            response.EnsureSuccessStatusCode();
            var employee = await GetResponseContent<EmployeeDto>(response);

            Assert.True(employee != null);
        }

        [Theory]
        // As manager
        [InlineData("b0b@yandex.ru", 2, HttpStatusCode.OK)]
        [InlineData("b0b@yandex.ru", 1, HttpStatusCode.OK)]
        [InlineData("b0b@yandex.ru", - 1, HttpStatusCode.NotFound)]
        // As employee
        [InlineData("eve.99@yandex.ru", 2, HttpStatusCode.Forbidden)]
        public async void GetLeavesAsync_GetWithAuthUser_ReturnsCollectionOfLeaveDto(string employeeEmail, int employeeId, HttpStatusCode statusCode)
        {
            var response = await GetResponse(employeeEmail, $"/api/employee/{employeeId}/leaves");

            Assert.True(response.StatusCode == statusCode);

            if (statusCode != HttpStatusCode.OK) return;

            response.EnsureSuccessStatusCode();
            var leaves = await GetResponseContent<ICollection<LeaveDto>>(response);
            if (employeeId == 1)
            {
                Assert.False(leaves.Any());
            }
            else
            {
                Assert.True(leaves.Any());
            }
        }

        private async Task<string> GetTokenAsync(string email)
        {
            var data = new LoginDto
            {
                Email = email,
                Password = $"{email}Password1"
            };
            var secretResponse = await _client.PostAsJsonAsync("api/account/login", data);
            secretResponse.EnsureSuccessStatusCode();
            var stringSecretResponse = await secretResponse.Content.ReadAsStringAsync();
            return stringSecretResponse.Substring(12, stringSecretResponse.Length - 14);
        }

        private async Task<HttpResponseMessage> GetResponse(string email, string url)
        {
            var stringSecretResponse = await GetTokenAsync(email);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", stringSecretResponse);

            return await _client.GetAsync(url);
        }

        private async Task<T> GetResponseContent<T>(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResponse);
        }
    }
}
