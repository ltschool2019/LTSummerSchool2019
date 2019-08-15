using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.Domain.Api
{
    public enum DayType
    {
        WorkingDay = 0,
        DayOff = 1,
        HalfHoliday = 2,
        ErrorInDate = 100,
        NotFound = 101,
        InternalServiceError = 199
    }
}
