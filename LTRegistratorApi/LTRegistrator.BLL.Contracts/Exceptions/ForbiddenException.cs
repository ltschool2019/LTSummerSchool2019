using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.BLL.Contracts.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
