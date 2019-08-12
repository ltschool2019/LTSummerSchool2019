using System.Threading.Tasks;
using LTRegistrator.BLL.Contracts.Models;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface IReportService
    {
        Task<HourReportBllModel> GetHourReportAsync(int managerId);
    }
}