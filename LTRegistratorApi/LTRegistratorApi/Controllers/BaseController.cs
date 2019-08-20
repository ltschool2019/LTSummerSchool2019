using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LTRegistratorApi.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        private UserManager<User> _userManager;
        private HttpContext _httpContext;
        private DbContext DbContext { get; set; }

        protected BaseController() { }
        protected BaseController(UserManager<User> userManager, HttpContext httpContext, DbContext db)
        {
            _userManager = userManager;
            _httpContext = httpContext;
            DbContext = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// The method returns true if the user tries to change his data or he is a manager or administrator.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Is it possible to change the data</returns>
        protected async Task<bool> AccessAllowed(int id)
        {
            var thisUser = await _userManager.FindByIdAsync(
                _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)); //We are looking for an authorized user.
            var authorizedUser =
                await DbContext.Set<Employee>().SingleOrDefaultAsync(
                    e => e.Id ==
                         thisUser.EmployeeId); //We load Employee table.
            var maxRole = authorizedUser.MaxRole;

            return authorizedUser.Id == id ||
                   maxRole == RoleType.Manager ||
                   maxRole == RoleType.Administrator;
        }
    }
}
