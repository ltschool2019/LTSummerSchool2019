using System;

namespace LTRegistrator.Domain.Entities.Base
{
    public interface IGuidId
    {
        Guid Id { get; set; }
    }
}