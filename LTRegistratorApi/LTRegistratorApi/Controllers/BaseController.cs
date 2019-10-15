using System;
using System.Security.Claims;
using AutoMapper;
using LTRegistratorApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi.Controllers
{
    [ApiController, GlobalApiException]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// DbContext class
        /// </summary>
        protected readonly DbContext Db;
        protected readonly IMapper Mapper;

        protected BaseController(DbContext db, IMapper mapper)
        {
            Db = db ?? throw new ArgumentNullException(nameof(db));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Current employee id
        /// </summary>
        protected int CurrentEmployeeId => Convert.ToInt32(User.FindFirstValue("EmployeeID"));
    }
}
