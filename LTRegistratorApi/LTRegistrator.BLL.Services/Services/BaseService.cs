using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Exceptions;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public abstract class BaseService
    {
        protected DbContext DbContext { get; set; }
        protected IMapper Mapper { get; set; }
        public BaseService(DbContext db, IMapper mapper)
        {
            DbContext = db ?? throw new ArgumentNullException(nameof(db));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected async Task<RoleType> GetRole(int employeeId)
        {
            var user = await DbContext.Set<User>().FirstOrDefaultAsync(u => u.EmployeeId == employeeId);
            if (user == null)
            {
                throw new NotFoundException("User was not found");
            }

            var claim = await DbContext.Set<IdentityUserClaim<string>>().Where(c => c.UserId == user.Id).FirstOrDefaultAsync(c => c.ClaimType == ClaimTypes.Role);
            if (claim != null)
            {
                return Enum.Parse<RoleType>(claim.ClaimValue, true);
            }

            throw new ForbiddenException("User doesn't contains claim role");
        }
    }
}
