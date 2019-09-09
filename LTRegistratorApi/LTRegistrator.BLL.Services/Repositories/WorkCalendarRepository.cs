using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LTRegistrator.BLL.Contracts.Contracts;

namespace LTRegistrator.BLL.Services.Repositories
{
    public class WorkCalendarRepository : IWorkCalendarRepository
    {
        // формат ответа: 00000110000011 
        // 0 - рабочий день
        // 1 - выходной
        private const string BaseUrl = "https://isdayoff.ru/api/getdata";
        private readonly HttpClient _httpClient;

        public WorkCalendarRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckDay(DateTime date)
        {
            var parameters = new Dictionary<string, object>
            {
                {"year", date.Year },
                {"month", date.Month },
                {"day", date.Day }
            };

            var result = await GetResponseResult(date, parameters).ConfigureAwait(false);

            return result.FirstOrDefault().Value;
        }

        public async Task<IDictionary<DateTime, bool>> GetWorkCalendarByMonth(DateTime date)
        {
            var parameters = new Dictionary<string, object>
            {
                { "year", date.Year },
                {"month", date.Month }
            };

            return await GetResponseResult(date, parameters).ConfigureAwait(false);
        }

        public async Task<int> GetCountOfWorkingDaysInMonth(DateTime date)
        {
            var parameters = new Dictionary<string, object>
            {
                { "year", date.Year },
                { "month", date.Month }
            };

            var response = await SendGetRequest(BaseUrl, parameters);
            response.EnsureSuccessStatusCode();

            var content = await GetResponseContent(response).ConfigureAwait(false);
            return content.Count(c => c);
        }

        public async Task<IDictionary<DateTime, bool>> GetCalendarByYear(DateTime date)
        {
            var parameters = new Dictionary<string, object>
            {
                { "year", date.Year }
            };

            return await GetResponseResult(date, parameters).ConfigureAwait(false);
        }

        private async Task<IDictionary<DateTime, bool>> GetResponseResult(DateTime startDate, IDictionary<string, object> parameters)
        {
            var response = await SendGetRequest(BaseUrl, parameters).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await GetResponseContent(response).ConfigureAwait(false);
            return AssociateResponseWithDate(startDate, content);
        }

        private async Task<HttpResponseMessage> SendGetRequest(string url, IDictionary<string, object> parameters)
        {
            var urlParams = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            url += urlParams.Length > 0 ? $"?{urlParams}" : string.Empty;
            return await _httpClient.GetAsync(url).ConfigureAwait(false);
        }

        private async Task<IEnumerable<bool>> GetResponseContent(HttpResponseMessage responseMessage)
        {
            var stringResponse = await responseMessage.Content.ReadAsStringAsync();
            return stringResponse.ToCharArray().Select(c => c.ToString() == "0");
        }

        private IDictionary<DateTime, bool> AssociateResponseWithDate(DateTime startDate, IEnumerable<bool> responseContent)
        {
            var result = new Dictionary<DateTime, bool>();
            foreach (var item in responseContent)
            {
                result.Add(startDate, item);
                startDate = startDate.AddDays(1);
            }

            return result;
        }
    }
}
