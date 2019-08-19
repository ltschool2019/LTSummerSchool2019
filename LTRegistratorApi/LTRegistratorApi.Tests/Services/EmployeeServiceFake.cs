using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Services.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;

namespace LTRegistratorApi.Tests.Services
{
    public class EmployeeServiceFake : IEmployeeService
    {
        private readonly List<Employee> _employees;
        private readonly IMapper _mapper;

        public EmployeeServiceFake(IMapper mapper)
        {
            _mapper = mapper;
            var leaveBob = new Leave[]
            {
                new Leave() { Id = 1, StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 2, StartDate = new DateTime(2019, 8, 6), EndDate = new DateTime(2019, 8, 8), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 3, StartDate = new DateTime(2019, 8, 1), EndDate = new DateTime(2019, 8, 4), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 4, StartDate = new DateTime(2019, 2, 10), EndDate = new DateTime(2019, 2, 13), TypeLeave = TypeLeave.SickLeave }
            };
            var leaveEve = new Leave[]
            {
                new Leave() { Id= 5, StartDate = new DateTime(2019, 2, 10), EndDate = new DateTime(2019, 3, 1), TypeLeave = TypeLeave.Training }
            };
            var leaveCarol = new Leave[]
            {
                new Leave() { Id = 6, StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 7, StartDate = new DateTime(2019, 2, 1), EndDate = new DateTime(2019, 2, 15), TypeLeave = TypeLeave.Training },
                new Leave() { Id = 8, StartDate = new DateTime(2019, 3, 1), EndDate = new DateTime(2019, 4, 1), TypeLeave = TypeLeave.SickLeave }
            };
            var leaveFrank = new Leave[]
            {
                new Leave() { Id = 9, StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Vacation },
                new Leave() { Id = 10, StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2019, 1, 13), TypeLeave = TypeLeave.Idle }
            };

            _employees = new List<Employee>()
            {
                new Employee() {Id=1, FirstName = "Alice", SecondName = "Brown", Mail = "alice@mail.ru", MaxRole = RoleType.Administrator, Rate = 1.5, Leaves = new List<Leave>()},
                new Employee() {Id=2, FirstName = "Bob", SecondName = "Johnson", Mail = "b0b@yandex.ru", MaxRole = RoleType.Manager, Leaves = leaveBob, Rate = 1 },
                new Employee() {Id=3, FirstName = "Eve", SecondName = "Williams", Mail = "eve.99@yandex.ru", MaxRole = RoleType.Employee, Leaves = leaveEve, Rate = 1.25, ManagerId = 2 },
                new Employee() {Id=4, FirstName = "Carol", SecondName = "Smith", Mail = "car0l@mail.ru", MaxRole = RoleType.Manager, Leaves = leaveCarol, Rate = 1 },
                new Employee() {Id=5, FirstName = "Dave", SecondName = "Jones", Mail = "dave.99@mail.ru", MaxRole = RoleType.Employee, Rate = 1, ManagerId = 2 ,Leaves = new List<Leave>()},
                new Employee() {Id=6, FirstName = "Frank", SecondName = "Florence", Mail = "frank.99@mail.ru", MaxRole = RoleType.Employee, Leaves = leaveFrank, Rate = 0.25, ManagerId = 4 }
        };
        }
        public async Task<Response<Employee>> GetByIdAsync(int id)
        {
            var employee = await Task<Employee>.Factory.StartNew(() =>
            {
                var result = _employees.FirstOrDefault(e => e.Id == id);
                return result;
            });

            if (employee == null)
            {
                return new Response<Employee>(HttpStatusCode.NotFound, $"Employee with id = {id} not found");
            }

            return new Response<Employee>(employee);
        }

        public async Task<Response<Employee>> AddLeavesAsync(int userId, ICollection<Leave> leaves)
        {
            var employee = await Task<Employee>.Factory.StartNew(() =>
            {
                var result = _employees.FirstOrDefault(e => e.Id == userId);
                return result;
            });

            if (employee == null)
            {
                return new Response<Employee>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            var index = _employees.SelectMany(e => e.Leaves).Max(l => l.Id) + 1;
            leaves = leaves.Select(l =>
            {
                var result = new Leave
                {
                    Id = index,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    TypeLeave = l.TypeLeave
                };
                index++;
                return result;
            }).ToList();

            if (LeavesValidator.TryMergeLeaves(employee.Leaves.ToList(), leaves))
            {
                employee.Leaves = employee.Leaves.Concat(leaves).ToList();

                return new Response<Employee>(employee);
            }

            return new Response<Employee>(HttpStatusCode.BadRequest, "Transferred leave is not correct");
        }

        public async Task<Response<Employee>> UpdateLeavesAsync(int userId, ICollection<Leave> leaves)
        {
            var employee = await Task<Employee>.Factory.StartNew(() =>
            {
                var result = _employees.FirstOrDefault(e => e.Id == userId);
                return result;
            });
            if (employee == null)
            {
                return new Response<Employee>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            try
            {
                var temLeaves = employee.Leaves.Select(l => _mapper.Map<Leave>(l)).ToList(); 
                foreach (Leave leave in leaves)
                {
                    leave.EmployeeId = employee.Id;
                    leave.Employee = employee;
                    var currentLeave = temLeaves.FirstOrDefault(l => l.Id == leave.Id);
                    
                    if (currentLeave == null)
                    {
                        return new Response<Employee>(HttpStatusCode.NotFound, $"Leave with id = {leave.Id} not found");
                    }
                    _mapper.Map(leave, currentLeave);
                }

                if (!LeavesValidator.ValidateLeaves(temLeaves))
                {
                    return new Response<Employee>(HttpStatusCode.BadRequest, "Transferred leave is not correct");
                }

                employee.Leaves = temLeaves;
            }
            catch (Exception e)
            {
                return new Response<Employee>(HttpStatusCode.InternalServerError, "Internal server error");
            }

            return new Response<Employee>(_mapper.Map<Employee>(employee));
        }

        public Task<Response<Employee>> DeleteLeavesAsync(int userId, ICollection<int> leaveIds)
        {
            throw new NotImplementedException();
        }
    }
}
