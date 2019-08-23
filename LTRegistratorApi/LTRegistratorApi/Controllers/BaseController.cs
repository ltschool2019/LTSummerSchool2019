using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// DbContext class
        /// </summary>
        protected readonly DbContext Db;

        protected BaseController(DbContext db)
        {
            Db = db ?? throw new ArgumentNullException(nameof(db));
        }
    }
}
