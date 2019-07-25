using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTRegistrator.DAL;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Output of all projects of the manager. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        readonly LTRegistratorDbContext _db;
        public ManagerController(LTRegistratorDbContext context)
        {
            _db = context;
        }
        //GET api/manager/1/projects
        [HttpGet("{EmployeeId}/projects")]
        public ActionResult<string> Get(Guid employeeId)
        {
            var result = _db.ProjectEmployee.Join(_db.Project,
                                     p => p.ProjectId,
                                     pe => pe.Id,
                                     (pe, p) => new { pe, p }).Where(w => w.pe.EmployeeId == employeeId && w.pe.Role == "Manager").Select(name => new { name.p.Name });
            if (User == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
