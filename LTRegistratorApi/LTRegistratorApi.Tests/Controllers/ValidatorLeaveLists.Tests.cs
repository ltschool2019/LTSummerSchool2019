using LTRegistratorApi.Controllers;
using LTRegistratorApi.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace LTRegistratorApi.Tests.Controllers
{
    public class ValidatorLeaveListsTests
    {
        [Theory]
        [InlineData(true, 2, 4)]
        [InlineData(false, 2, 2)]
        [InlineData(false, 4, 2)]
        public void SimpleTestForOneList(bool expected, int start, int end)
        {
            var list = new List<Leave>() { new Leave { StartDate = new DateTime(2019, 1, start), EndDate = new DateTime(2019, 1, end) } };
            Assert.Equal(expected, ValidatorLeaveLists.ValidateLeaves(list));
        }

        [Theory]
        [InlineData(true, 2, 4, 5, 7)]
        [InlineData(true, 5, 7, 2, 4)]
        [InlineData(true, 2, 5, 5, 7)]
        [InlineData(false, 2, 5, 3, 7)]
        [InlineData(false, 2, 5, 3, 5)]
        [InlineData(false, 2, 4, 3, 4)]
        public void NormalTestForOneList(bool expected, int start1, int end1, int start2, int end2)
        {
            var list = new List<Leave>() {
                new Leave { StartDate = new DateTime(2019, 1, start1), EndDate = new DateTime(2019, 1, end1) },
                new Leave { StartDate = new DateTime(2019, 1, start2), EndDate = new DateTime(2019, 1, end2) }
            };
            Assert.Equal(expected, ValidatorLeaveLists.ValidateLeaves(list));
        }

        [Theory]
        [InlineData(true, 2, 4, 5, 7, 8, 9)]
        [InlineData(false, 2, 4, 5, 7, 9, 8)]
        [InlineData(true, 1, 2, 2, 3, 3, 4)]
        [InlineData(false, 1, 2, 2, 4, 3, 4)]
        public void ComplexTestForOneList(bool expected, int start1, int end1, int start2, int end2, int start3, int end3)
        {
            var list = new List<Leave>() {
                new Leave { StartDate = new DateTime(2019, 1, start1), EndDate = new DateTime(2019, 1, end1) },
                new Leave { StartDate = new DateTime(2019, 1, start2), EndDate = new DateTime(2019, 1, end2) },
                new Leave { StartDate = new DateTime(2019, 1, start3), EndDate = new DateTime(2019, 1, end3) }
            };
            Assert.Equal(expected, ValidatorLeaveLists.ValidateLeaves(list));
        }

        [Theory]
        [InlineData(true, 2, 4, 5, 7, 8, 9)]
        [InlineData(false, 2, 4, 5, 7, 9, 8)]
        [InlineData(true, 1, 2, 2, 3, 3, 4)]
        [InlineData(false, 1, 2, 2, 4, 3, 4)]
        public void TestForTwoLists(bool expected, int start1, int end1, int start2, int end2, int start3, int end3)
        {
            var first = new List<Leave>() {
                new Leave { StartDate = new DateTime(2019, 1, start1), EndDate = new DateTime(2019, 1, end1) },
                new Leave { StartDate = new DateTime(2019, 1, start2), EndDate = new DateTime(2019, 1, end2) }
            };
            var second = new List<Leave>() {
                new Leave { StartDate = new DateTime(2019, 1, start3), EndDate = new DateTime(2019, 1, end3) }
            };
            Assert.Equal(expected, ValidatorLeaveLists.MergingListsValidly(first, second));
        }
    }
}
