using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Mvc;
using LTRegistratorApi.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

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
        /// GET api/employee/{id}/info
        /// Sends user information.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>Basic Employee information</returns>
        [HttpGet("{id}/info")]
        public ActionResult<EmployeeDto> GetInfo(int id) 
            => db.Employee.Select(e => new EmployeeDto
            {
                EmployeeId = e.EmployeeId,
                FirstName = e.FirstName,
                SecondName = e.SecondName,
                Mail = e.Mail,
                MaxRole = e.MaxRole,
                Projects = ToProjectDto(e.ProjectEmployee.Select(ep => ep.Project).ToList())
            }).SingleOrDefault(V => V.EmployeeId == id);

        private static List<ProjectDto> ToProjectDto(List<Project> projects)
        {
            var result = new List<ProjectDto>();
            foreach (var project in projects)
                result.Add(new ProjectDto { ProjectId = project.ProjectId, Name = project.Name });

            return result;
        }


        /// <summary>
        /// GET api/employee/{id}/leaves
        /// Gets a list of all human leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>User's leave list</returns>
        [HttpGet("{id}/leaves")]
        public ActionResult<List<Leave>> GetLeaves(int id)
        {
            var user = db.Employee
                .Include(e => e.Leave)
                .SingleOrDefault(V => V.EmployeeId == id);

            if (user == null) return NotFound();

            return user.Leave.ToList();
        }

        /// <summary>
        /// POST api/employee/{id}/leaves
        /// Add new leaves for user.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leavesDto">List of LeaveDto that is added to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPost("{id}/leaves")]
        public ActionResult SetLeaves(int id, [FromBody] List<LeaveDto> leavesDto)
        {
            var user = db.Employee
                .Include(e => e.Leave)
                .SingleOrDefault(V => V.EmployeeId == id);

            if (leavesDto != null && user != null)
            {
                var leaves = new List<Leave>();
                foreach (var leave in leavesDto)
                    leaves.Add(new Leave { TypeLeave = leave.TypeLeave, StartDate = leave.StartDate, EndDate = leave.EndDate });

                if (ValidatorLeaveLists.MergingListsValidly(leaves, user.Leave.ToList()))
                    foreach (var leave in leaves)
                        user.Leave.Add(leave);
                else return BadRequest();
            }
            else return NotFound();

            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// PUT api/employee/{id}/leaves
        /// Updates information on leaves.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that updated</param>
        /// <returns>Was the operation successful?</returns>
        [HttpPut("{id}/leaves")]
        public ActionResult UpdateLeaves(int id, [FromBody] List<Leave> leaves)
        {
            var user = db.Employee
                .Include(e => e.Leave)
                .Single(V => V.EmployeeId == id);

            if (leaves != null && user != null)
            {
                foreach (var leave in leaves)
                {
                    var temp = user.Leave.SingleOrDefault(li => li.LeaveId == leave.LeaveId);
                    if (temp != null)
                    {
                        temp.StartDate = leave.StartDate;
                        temp.EndDate = leave.EndDate;
                        temp.TypeLeave = leave.TypeLeave;
                        db.Leave.Update(temp);
                    }
                    else return BadRequest();
                }

                if (!ValidatorLeaveLists.ValidateLeaves(user.Leave.ToList()))
                    return BadRequest();
            }
            else return NotFound();

            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// DELETE api/employee/{id}/leaves
        /// Deletes a leaves record.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="leaves">List of leaves that is deleted to the user</param>
        /// <returns>Was the operation successful?</returns>
        [HttpDelete("{id}/leaves")]
        public ActionResult DeleteLeave(int id, [FromBody] List<Leave> leaves)
        {
            var user = db.Employee
                .Include(l => l.Leave)
                .SingleOrDefault(E => E.EmployeeId == id);

            if (leaves != null && user != null)
                foreach (var leave in leaves)
                {
                    var temp = user.Leave.SingleOrDefault(li => li.LeaveId == leave.LeaveId);
                    if (temp != null)
                        db.Leave.Remove(temp);
                    else return BadRequest();
                }
            else return NotFound();

            db.SaveChanges();
            return Ok();
        }
    }

    /// <summary>
    /// Verifies that the dates in the List<Leave>('s) are correct.
    /// </summary>
    public static class ValidatorLeaveLists
    {
        public static bool MergingListsValidly(List<Leave> first, List<Leave> second) 
            => ValidateLeaves(first.Concat(second).ToList());

        public static bool ValidateLeaves(List<Leave> list)
        {
            var normalizedList = GetDoubleDTList(list);
            return normalizedList.StartEndIsValid() && normalizedList.LocationStartEndIsValid();
        }

        private static List<(DateTime, DateTime)> GetDoubleDTList(List<Leave> leaves)
        {
            if (leaves == null) throw new ArgumentNullException();

            var result = new List<(DateTime, DateTime)>();
            foreach (var leave in leaves)
                result.Add((leave.StartDate, leave.EndDate));

            return result;
        }

        private static bool StartEndIsValid(this List<(DateTime, DateTime)> list)
        {
            if (list == null) throw new ArgumentNullException();

            foreach (var item in list)
                if (item.Item1 >= item.Item2)
                    return false;

            return true;
        }

        private static bool LocationStartEndIsValid(this List<(DateTime, DateTime)> list)
        {
            if (list == null) throw new ArgumentNullException();

            list.Sort();
            for (int i = 0; i < list.Count() - 1; ++i)
                if (list[i].Item2 > list[i + 1].Item1)
                    return false;

            return true;
        }
    }
}
