﻿namespace DomainCommons
{
    public interface IHasDeletionTime
    {
        DateTimeOffset? DeletionTime { get; }
    }
}
