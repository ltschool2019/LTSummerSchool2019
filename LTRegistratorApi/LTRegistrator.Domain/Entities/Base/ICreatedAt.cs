using System;

namespace LTRegistrator.Domain.Entities.Base
{
    public interface ICreatedAt
    {
        DateTime CreatedAt { get; set; }
    }
}