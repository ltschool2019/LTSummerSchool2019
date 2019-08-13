using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using LTRegistrator.BLL.Contracts.Models;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IReportService
    {
        Task<HourReportBllModel> GetMonthlyReportAsync(int managerId, DateTime date);
    }
}