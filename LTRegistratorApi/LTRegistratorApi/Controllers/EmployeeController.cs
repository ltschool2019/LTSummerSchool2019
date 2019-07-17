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
        /// GET api/employee/{id}
        /// Gets a list of all human leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>User's leave list</returns>
        [HttpGet("{id}")]
        public ActionResult<List<Leave>> GetLeaves(int id)
        {
            var leaves = db.Employee.Include(e => e.Leave).SingleOrDefault(V => V.EmployeeId == id).Leave;

            if (leaves == null)
            {
                return BadRequest();
            }
            return leaves.ToList();
        }

        /// <summary>
        /// PUT api/employee/{id}/leaves/
        /// Add new leaves for user.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leave that is added to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPut("{id}/leaves")]
        public ActionResult SetLeaves(int id, [FromBody] List<Leave> leaves)
        {
            var user = db.Employee.Include(e => e.Leave).SingleOrDefaultAsync(V => V.EmployeeId == id);

            if (leaves != null || user.IsCompleted)
                foreach (var leave in leaves)
                    if (!user.Result.Leave.Contains(leave))
                        user.Result.Leave.Add(leave);
                    else return BadRequest();
            else return BadRequest();

            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// PUT api/employee/{id}/leaves/
        /// Updates information on leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that updated</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPut("{id}/leaves")]
        public ActionResult UpdateLeaves(int id, [FromBody] List<Leave> leaves)
        {
            var user = db.Employee.Include(e => e.Leave).SingleOrDefaultAsync(V => V.EmployeeId == id);

            if (leaves != null || user.IsCompleted)
            {
                foreach (var leave in leaves)
                {
                    var count = user.Result.Leave.Count();
                    db.Leave.Remove(db.Leave.SingleOrDefault(k => k.LeaveId == leave.LeaveId));
                    if (count == user.Result.Leave.Count()) //=> LeaveId not-> user
                        return BadRequest();

                    db.Leave.Add(leave);
                }
            }
            else return BadRequest();

            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// DELETE api/employee/{id}/leaves/
        /// Deletes a leaves record.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that is deleted to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpDelete("{id}/leaves")]
        public ActionResult DeleteLeave(int id, [FromBody] List<Leave> leaves)
        {
            var user = db.Employee.Include(e => e.Leave).SingleOrDefaultAsync(V => V.EmployeeId == id);

            if (leaves != null || user.IsCompleted || user.Result.Leave.Any())
            {
                foreach (var leave in leaves)
                    if (!user.Result.Leave.Remove(leave))
                        return BadRequest();
            }
            else return BadRequest();

            db.SaveChanges();
            return Ok();
        }
    }
}
