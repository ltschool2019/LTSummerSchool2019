using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        ApplicationContext db;
        public ManagerController(ApplicationContext context)
        {
            db = context;
        }
        // GET api/manager/Bob
        [HttpGet("{User}")]

        public ActionResult<string> Get(string User)
        {

            //IList<string> projectList = new List<string>();
            //var query = (from e in db.Employee
            //             join p in db.Project
            //             on e.EmployeeID equals p.ManagerId
            //             where e.User == User
            //             select new
            //             {
            //                 e.User,
            //                 p.Name
            //             });

            var result = db.Project.Join(db.Employee,
                                     p => p.ManagerId,
                                     e => e.EmployeeId,
                                     (p, e) => new { p, e }).Where(u => u.e.User == User).Select(u => new { u.p.Name });
                                                                                                        
                if (User == null)
                {
                    return BadRequest();
                }

                return result;
            
        }

    }
}
