using System;

namespace DomainCommons
{
    public interface IHasCreationTime
    {
        DateTimeOffset CreationTime { get; }
    }
}
