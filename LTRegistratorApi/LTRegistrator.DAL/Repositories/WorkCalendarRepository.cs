using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LTRegistrator.DAL.Contracts;
using System.Net.Http;

namespace LTRegistrator.DAL.Repositories
{
    public class WorkCalendarRepository : IWorkCalendarRepository
    {
        private const string BaseUrl = "https://isdayoff.ru";
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
    }
}
