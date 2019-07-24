using LTRegistratorApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LTRegistratorApi.Validators
{
    /// <summary>
    /// Verifies that the dates in the List<Leave>('s) are correct.
    /// </summary>
    public static class ValidatorLeaveLists
    {
        /// <summary>
        /// Check intersection of DateTime periods in lists.
        /// </summary>
        /// <param name="first">First list</param>
        /// <param name="second">Second list</param>
        /// <returns>Periods do not overlap and are the time periods correct?</returns>
        public static bool MergingListsValidly(List<Leave> first, List<Leave> second)
            => ValidateLeaves(first.Concat(second).ToList());

        /// <summary>
        /// Check intersection of DateTime periods in list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Periods do not overlap and are the time periods correct?</returns>
        public static bool ValidateLeaves(List<Leave> list)
            => GetDoubleDTList(list).LocationStartEndIsValid();

        /// <summary>
        /// Retrieves time information.
        /// </summary>
        /// <param name="leaves">Data list</param>
        /// <returns>List(start, end)</returns>
        private static List<(DateTime, DateTime)> GetDoubleDTList(List<Leave> leaves)
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
