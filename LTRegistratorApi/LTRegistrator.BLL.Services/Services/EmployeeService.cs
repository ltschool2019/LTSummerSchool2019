using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Dtos;
using LTRegistrator.DAL.Contracts;
using LTRegistrator.Domain.Entities;

namespace LTRegistrator.BLL.Services.Services
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        public EmployeeService(IUnitOfWork uow, IMapper mapper) : base(uow, mapper)
        {
        }

        public async Task<Response<EmployeeDto>> GetByIdAsync(Guid id)
        {
            var employee = await UnitOfWork.GetRepository<Employee>().FindByIdAsync(id);
            return employee == null 
                ? new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {id} not found") 
                : new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
        }

        public async Task<Response<EmployeeDto>> AddLeavesAsync(Guid userId, ICollection<LeaveDto> leaves)
        {
            var employee = await UnitOfWork.GetRepository<Employee>().FindByIdAsync(userId);
            if (employee == null)
            {
                return new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }
            var newLeaves = Mapper.Map<ICollection<Leave>>(leaves).ToList();

            if (LeavesValidator.TryMergeLeaves(employee.Leaves.ToList(), newLeaves))
            {
                employee.Leaves = employee.Leaves.Concat(newLeaves).ToList();
                await UnitOfWork.CommitAsync();

                return new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
            }

            return new Response<EmployeeDto>(HttpStatusCode.BadRequest, "");
        }

        public async Task<Response<EmployeeDto>> UpdateLeavesAsync(Guid userId, ICollection<LeaveDto> leaves)
        {
            var employee = await UnitOfWork.GetRepository<Employee>().FindByIdAsync(userId);
            if (employee == null)
            {
                return new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            var entities = Mapper.Map<ICollection<Leave>>(leaves);
            try
            {
                foreach (var entity in entities)
                {
                    entity.EmployeeId = userId;
                    UnitOfWork.GetRepository<Leave>().Update(entity);
                }

                await UnitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                return new Response<EmployeeDto>(HttpStatusCode.InternalServerError, "Internal server error");
            }

            return new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
        }

        public async Task<Response<EmployeeDto>> DeleteLeavesAsync(Guid userId, ICollection<LeaveDto> leaves)
        {
            var employee = await UnitOfWork.GetRepository<Employee>().FindByIdAsync(userId);
            if (employee == null)
            {
                return new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            if (leaves.Any(l => l.EmployeeId != userId && l.EmployeeId != Guid.Empty))
            {
                return new Response<EmployeeDto>(HttpStatusCode.Forbidden, "You cannot change another employee’s leave");
            }

            var leaveIds = leaves.Select(l => l.Id);
            var selectedLeaves = (from leave in employee.Leaves
                                  where leaveIds.Contains(leave.Id)
                                  select leaves).ToList();
            if (selectedLeaves.Count != leaves.Count)
            {
                return new Response<EmployeeDto>(HttpStatusCode.BadRequest, "Transferred leave is not correct");
            }

            UnitOfWork.GetRepository<Leave>().Remove(selectedLeaves);
            await UnitOfWork.CommitAsync();
            return new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
        }
    }
}
