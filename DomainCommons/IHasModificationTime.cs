namespace DomainCommons
{
    public interface IHasModificationTime
    {
        DateTimeOffset? LastModificationTime { get; }

    }
}
