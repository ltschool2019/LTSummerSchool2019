using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTRegistrator.Domain.Entities;

namespace LTRegistrator.BLL.Services.Services
{
    public static class LeavesValidator
    {
        /// <summary>
        /// Check intersection of DateTime periods in lists.
        /// </summary>
        /// <param name="first">First list</param>
        /// <param name="second">Second list</param>
        /// <returns>Periods do not overlap and are the time periods correct?</returns>
        public static bool TryMergeLeaves(ICollection<Leave> first, ICollection<Leave> second)
            => ValidateLeaves(first.Concat(second).ToList());

        /// <summary>
        /// Check intersection of DateTime periods in list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Periods do not overlap and are the time periods correct?</returns>
        public static bool ValidateLeaves(ICollection<Leave> list)
            => GetDoubleDtList(list).LocationStartEndIsValid();

        /// <summary>
        /// Retrieves time information.
        /// </summary>
        /// <param name="leaves">Data list</param>
        /// <returns>List(start, end)</returns>
        private static List<(DateTime, DateTime)> GetDoubleDtList(ICollection<Leave> leaves)
        {
            if (leaves == null) throw new ArgumentNullException();

            var result = new List<(DateTime, DateTime)>();
            foreach (var leave in leaves)
                result.Add((leave.StartDate, leave.EndDate));

            return result;
        }

        /// <summary>
        /// Checks the correctness of the location of the periods.
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Periods do not overlap and are the time periods correct?</returns>
        private static bool LocationStartEndIsValid(this List<(DateTime, DateTime)> list)
        {
            if (list == null) throw new ArgumentNullException();

            //Checks the correctness of the location of the start and end
            foreach (var item in list)
                if (item.Item1 >= item.Item2)
                    return false;

            //Checks the correctness of the location of the correct period
            list.Sort();
            for (int i = 0; i < list.Count() - 1; ++i)
                if (list[i].Item2 > list[i + 1].Item1)
                    return false;

            return true;
        }
    }
}
