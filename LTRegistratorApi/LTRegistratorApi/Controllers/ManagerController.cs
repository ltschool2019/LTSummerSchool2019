using LTRegistratorApi.Model;
using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Output of all projects of the manager. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController, Authorize(Policy = "IsManagerOrAdministrator")]
    public class ManagerController : ControllerBase
    {
        ApplicationContext db;
        public ManagerController(ApplicationContext context)
        {
            db = context;
        }
        // GET api/manager/1/projects
        [HttpGet("{EmployeeId}/projects")]
        public ActionResult<string> Get(int EmployeeId)
        {
            var result = db.ProjectEmployee.Join(db.Project,
                                     p => p.ProjectId,
                                     pe => pe.ProjectId,
                                     (pe, p) => new { pe, p }).Where(w => w.pe.EmployeeId == EmployeeId && w.pe.Role == RoleType.Manager).Select(name => new { name.p.Name });
            if (User == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
