using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.BLL.Contracts.Models
{
    public class HourReportBllModel
    {
        public IEnumerable<HourReportUserBllModel> Users { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }

    public class HourReportUserBllModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string LastName { get; set; }
        public double Rate { get; set; }
        public double NormHours { get; set; }
        public IEnumerable<HourReportProjectBllModel> Projects { get; set; }
        public IEnumerable<HourReportLeaveBllModel> Leaves { get; set; }
    }

    public class HourReportProjectBllModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public double Hours { get; set; }
        public int ManagerId { get; set; }
    }

    public class HourReportLeaveBllModel
    {
        public EventType EventType { get; set; }
        public string Name { get; set; }
        public double Hours { get; set; }
    }

    public class Event
    {
        public string Name { get; set; }
        public EventType EventType { get; set; }
    }

    public enum EventType
    {
        ManagerProject,
        Vacation,
        AnotherLeave,
        NotManagerProject
    }
}
