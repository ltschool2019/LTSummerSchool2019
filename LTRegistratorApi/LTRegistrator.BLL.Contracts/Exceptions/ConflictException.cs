using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.BLL.Contracts.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message): base(message) { }
    }
}
