using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.BLL.Contracts.Models;
using LTRegistrator.Domain.Enums;

namespace LTRegistrator.BLL.Services
{
    public static class EnumAssociations
    {
        public static readonly IDictionary<TypeLeave, string> LeaveNames = new Dictionary<TypeLeave, string>
        {
            { TypeLeave.Vacation, "Оплачиваемый отпуск"},
            { TypeLeave.SickLeave, "Больничный" },
            { TypeLeave.Idle, "Скамейка без проекта" },
            { TypeLeave.Training, "Обучение" }
        };

        public static readonly IDictionary<TypeLeave, EventType> LeaveEventTypes = new Dictionary<TypeLeave, EventType>
        {
            {TypeLeave.Vacation, EventType.Vacation },
            {TypeLeave.SickLeave, EventType.AnotherLeave },
            {TypeLeave.Idle, EventType.AnotherLeave },
            {TypeLeave.Training, EventType.AnotherLeave },
        };
    }
}
