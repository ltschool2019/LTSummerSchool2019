using System.Collections.Generic;
using System.Threading.Tasks;
using LTRegistrator.BLL.Contracts.Models.CustomFields;

namespace LTRegistrator.BLL.Contracts.Contracts
{
    public interface ICustomFieldService
    {
        Task<IEnumerable<CustomFieldBllModel>> GetByProjectIdAsync(int projectId);
        Task<CustomFieldBllModel> GetByIdAsync(int id);
        Task AddAsync(CustomFieldBllModel customField);
        Task UpdateAsync(CustomFieldBllModel customField);
        Task RemoveAsync(int id);
    }
}