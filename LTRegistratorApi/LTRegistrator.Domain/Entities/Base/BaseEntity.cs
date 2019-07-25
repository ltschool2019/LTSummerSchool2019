using System;
using System.Collections.Generic;
using System.Text;

namespace LTRegistrator.Domain.Entities.Base
{
    public abstract class BaseEntity : IGuidId, ICreatedAt, IModifiedAt
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
