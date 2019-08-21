using System;
using System.Security.Claims;
using System.Threading.Tasks;
using LTRegistrator.BLL.Services;
using LTRegistrator.Domain.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private LTRegistratorDbContext _dbContext;

        protected BaseController() { }
        protected BaseController(LTRegistratorDbContext db)
        {
            _dbContext = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// The method returns true if the user tries to change his data or he is a manager or administrator.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Is it possible to change the data</returns>
        protected async Task<bool> AccessAllowed(int id)
        {
            var userId = Convert.ToInt32(User.Identity.GetUserId());
            var employee = await _dbContext.Employee.SingleOrDefaultAsync(e => e.Id == userId);
            var maxRole = employee.MaxRole;

            return userId == id ||
                   maxRole == RoleType.Manager ||
                   maxRole == RoleType.Administrator;
        }
    }
}
