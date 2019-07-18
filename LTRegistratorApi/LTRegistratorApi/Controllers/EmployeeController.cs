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
        /// POST api/employee/{id}/leaves/
        /// POST LeaveId = -1
        /// Add new leaves for user.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leave that is added to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPost("{id}/leaves")]
        public ActionResult SetLeaves(int id, [FromBody] List<Leave> leaves)
        {
            var user = db.Employee.Include(e => e.Leave).SingleOrDefault(V => V.EmployeeId == id);

            if (leaves != null)
                foreach (var leave in leaves)
                {
                    var temp = user.Leave.Where(li => li.LeaveId == leave.LeaveId);
                    if (temp.Count() == 0)
                    {
                        var newLeave = new Leave() { StartDate = leave.StartDate, EndDate = leave.EndDate, TypeLeave = leave.TypeLeave };
                        user.Leave.Add(newLeave);
                    }
                    else return BadRequest();
                }
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
            var user = db.Employee.Include(e => e.Leave).Single(V => V.EmployeeId == id);

            if (leaves != null)
            {
                foreach (var leave in leaves)
                {
                    var temp = user.Leave.SingleOrDefault(li => li.LeaveId == leave.LeaveId);
                    if (temp != null)
                        db.Leave.Remove(temp);
                    else return BadRequest();
                    var newTemp = user.Leave.Where(li => li.LeaveId == leave.LeaveId);
                    if (newTemp.Count() == 1)
                    {
                        var newLeave = new Leave() { StartDate = leave.StartDate, EndDate = leave.EndDate, TypeLeave = leave.TypeLeave };
                        user.Leave.Add(newLeave);
                    }
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
            var user = db.Employee.Include(l => l.Leave).SingleOrDefault(E => E.EmployeeId == id);

            if (leaves != null)
                foreach (var leave in leaves)
                {
                    var temp = user.Leave.SingleOrDefault(li => li.LeaveId == leave.LeaveId);
                    if (temp != null)
                        db.Leave.Remove(temp);
                    else return BadRequest();
                }
            else return BadRequest();

            db.SaveChanges();
            return Ok();
        }
    }
}
