using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Xunit;

namespace LTRegistratorApi.Tests.Integrations
{
    public class BaseControllerTest : IClassFixture<CustomWebApplicationFactory<FakeStartup>>
    {
        protected readonly HttpClient Client;
        protected readonly IWebHost Host;

        public BaseControllerTest(CustomWebApplicationFactory<FakeStartup> factory)
        {
            Client = factory.CreateClient(factory.ClientOptions);
            Host = factory.Server.Host;
        }

        protected async Task<HttpResponseMessage> GetResponse(string email, string url)
        {
            var stringSecretResponse = await GetTokenAsync(email);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", stringSecretResponse);

            return await Client.GetAsync(url);
        }

        protected async Task<HttpResponseMessage> SendResponseWithBody(HttpMethod method, string email, string url, object bodyObject)
        {
            var stringSecretResponse = await GetTokenAsync(email);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", stringSecretResponse);

            using (var request = new HttpRequestMessage(method, url))
            using (var httpContent = CreateHttpContent(bodyObject))
            {
                request.Content = httpContent;
                return await Client.SendAsync(request).ConfigureAwait(false);
            }
        }

        protected async Task<T> GetResponseContent<T>(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResponse);
        }

        private async Task<string> GetTokenAsync(string email)
        {
            var data = new LoginDto
            {
                Email = email,
                Password = $"{email}Password1"
            };
            var secretResponse = await Client.PostAsJsonAsync("api/account/login", data);
            secretResponse.EnsureSuccessStatusCode();
            var stringSecretResponse = await secretResponse.Content.ReadAsStringAsync();
            return stringSecretResponse.Substring(12, stringSecretResponse.Length - 14);
        }
        
        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        private static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
    }
}
