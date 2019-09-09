using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IWorkCalendarRepository
    {
        Task<bool> CheckDay(DateTime date);
        Task<IDictionary<DateTime, bool>> GetWorkCalendarByMonth(DateTime date);
        Task<int> GetCountOfWorkingDaysInMonth(DateTime date);
        Task<IDictionary<DateTime, bool>> GetCalendarByYear(DateTime date);
    }
}