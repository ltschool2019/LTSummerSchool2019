using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.Domain.Entities.Base
{
    public abstract class BaseEntity : IIntId
    {
        public int Id { get; set; }
    }
}
