using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Controller, allowing you to manage leaves (vocations).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        ApplicationContext db;
        public EmployeeController(ApplicationContext context)
        {
            db = context;
        }

        /// <summary>
        /// GET api/employee
        /// Gets a list of all people with leaves.
        /// </summary>
        /// <returns>List of people with leaves</returns>
        [HttpGet]
        public ActionResult<List<(string email, int id, List<Leave>)>> GetLeaves()
        {
            var users = db.Employee.Where(V => V.Leave != null);

            if (users == null)
            {
                return BadRequest();
            }
            var result = new List<(string, int, List<Leave>)>();

            foreach (var user in users)
            {
                result.Add((user.ApplicationUser.Email, user.EmployeeID, user.Leave.ToList()));
            }
            return result;
        }

        /// <summary>
        /// GET api/employee/id
        /// Gets a list of all human leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>User's leave list</returns>
        [HttpGet("{id}")]
        public ActionResult<List<Leave>> GetLeaves(int id)
        {
            var leaves = db.Leave.Where(V => V.EmployeeId == id);

            if (leaves == null)
            {
                return BadRequest();
            }
            return leaves.ToList();
        }

        /// <summary>
        /// PUT api/employee/setleaves/id
        /// Add new leaves for user.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leave that is added to the user</param>
        [HttpPut("setleaves/{id}")]
        public void SetLeaves(int id, [FromBody] List<Leave> leaves)
        {
            var user = db.Employee.SingleOrDefaultAsync(V => V.EmployeeID == id);

            if (leaves != null || user.IsCompleted)
                foreach(var leave in leaves)
                    if (!user.Result.Leave.Contains(leave))
                        user.Result.Leave.Add(leave);
        }

        /// <summary>
        /// POST api/employee/editleaves/id
        /// Updates information on leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that fist is deleted, second added</param>
        [HttpPost("editleaves/{id}")]
        public void UpdateLeaves(int id, [FromBody] List<(Leave, Leave)> leaves)
        {
            var user = db.Employee.SingleOrDefaultAsync(V => V.EmployeeID == id);

            if (leaves != null || user.IsCompleted || user.Result.Leave.Any())
                foreach(var leave in leaves)
                {
                    user.Result.Leave.Remove(leave.Item1);
                    user.Result.Leave.Add(leave.Item2);
                }
        }

        /// <summary>
        /// DELETE api/employee/deleteleaves/id
        /// Deletes a leaves record.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that is deleted to the user</param>
        [HttpDelete("{deleteleave/id}")]
        public void DeleteLeave(int id, [FromBody] List<Leave> leaves)
        {
            var user = db.Employee.SingleOrDefaultAsync(V => V.EmployeeID == id);
            if (leaves != null || user.IsCompleted || user.Result.Leave.Any())
                foreach (var leave in leaves)
                    user.Result.Leave.Remove(leave);
        }
    }
}
