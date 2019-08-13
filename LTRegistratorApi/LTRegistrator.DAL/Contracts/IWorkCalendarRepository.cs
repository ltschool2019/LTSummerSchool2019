using System;
using System.Threading.Tasks;

namespace LTRegistrator.DAL.Contracts
{
    public interface IWorkCalendarRepository
    {
        Task<bool> CheckDay(DateTime date);
        Task GetWorkCalendarByMonth(DateTime date);
    }
}