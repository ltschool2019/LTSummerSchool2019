using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTRegistrator.DAL.Contracts;
using System.Net.Http;
using LTRegistrator.Domain.Api;

namespace LTRegistrator.DAL.Repositories
{
    public class WorkCalendarRepository : IWorkCalendarRepository
    {
        private const string BaseUrl = "https://isdayoff.ru/api/getdata";
        private readonly HttpClient _httpClient;
        public WorkCalendarRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckDay(DateTime date)
        {
            var url = $"{BaseUrl}?year={date.Year}&month={date.Month}&day={date.Day}";
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return true;
        }

        public async Task GetWorkCalendarByMonth(DateTime date)
        {
            var url = $"{BaseUrl}?year={date.Year}&month={date.Month}";
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<int> GetCountOfWorkingDaysInMonth(DateTime date)
        {
            var parameters = new Dictionary<string, object>
            {
                { "year", date.Year },
                { "month", date.Month }
            };

            try
            {
                var response = await SendGetRequest(BaseUrl, parameters);
                var result = response.ToCharArray().Select(c => (DayType) Convert.ToInt32(c.ToString())).Count(dt => dt == DayType.WorkingDay);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<string> SendGetRequest(string url, IDictionary<string, object> parameters)
        {
            url += "?";
            url = parameters.Aggregate(url, (current, parameter) => current + $"{parameter.Key}={parameter.Value}&");
            url = url.Remove(url.Length - 1);
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return responseContent;
        }
    }
}
