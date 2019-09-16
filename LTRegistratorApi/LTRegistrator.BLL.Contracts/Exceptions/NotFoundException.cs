using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.BLL.Contracts.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string message): base(message) { }
    }
}
