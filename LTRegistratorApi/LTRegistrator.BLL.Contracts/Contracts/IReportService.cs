using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LTRegistrator.BLL.Contracts.Models;
using LTRegistrator.Domain.Entities;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IReportService
    {
        Task<HourReportBllModel> GetMonthlyReportAsync(int managerId, DateTime date);
    }
}
