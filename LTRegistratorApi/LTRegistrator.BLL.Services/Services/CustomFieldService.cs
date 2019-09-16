using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Exceptions;
using LTRegistrator.BLL.Contracts.Models.CustomFields;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace LTRegistrator.BLL.Services.Services
{
    public class CustomFieldService: BaseService, ICustomFieldService
    {
        public CustomFieldService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }

        public async Task<IEnumerable<CustomFieldBllModel>> GetByProjectIdAsync(int projectId)
        {
            var customFields = await DbContext.Set<CustomField>()
                .Where(cf => cf.Projects.Select(p => p.Id).Contains(projectId)).ToListAsync();

            return Mapper.Map<IEnumerable<CustomFieldBllModel>>(customFields);
        }

        public async Task<CustomFieldBllModel> GetByIdAsync(int id)
        {
            var customField = await DbContext.Set<CustomField>().FindAsync(id);
            if (customField == null)
            {
                throw new NotFoundException("Custom field not found");
            }

            return Mapper.Map<CustomFieldBllModel>(customField);
        }

        public async Task AddAsync(CustomFieldBllModel customFieldDto)
        {
            var customField = Mapper.Map<CustomField>(customFieldDto);
            DbContext.Set<CustomField>().Add(customField);

            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomFieldBllModel customFieldDto)
        {
            var customField = await DbContext.Set<CustomField>().FindAsync(customFieldDto.Id);
            if (customField == null)
            {
                throw new NotFoundException("Custom field not found");
            }

            Mapper.Map(customFieldDto, customField);
            await DbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            var customField = await DbContext.Set<CustomField>().FindAsync(id);
            if (customField == null)
            {
                throw new NotFoundException("Custom field not found");
            }

            DbContext.Set<CustomField>().Remove(customField);
            await DbContext.SaveChangesAsync();
        }
    }
}
