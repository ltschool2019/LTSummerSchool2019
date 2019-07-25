using System;

namespace LTRegistrator.Domain.Entities.Base
{
    public interface IModifiedAt
    {
        DateTime ModifiedAt { get; set; }
    }
}