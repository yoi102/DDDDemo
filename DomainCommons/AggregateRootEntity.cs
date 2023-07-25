namespace DomainCommons
{
    public record AggregateRootEntity : BaseEntity, IAggregateRoot, ISoftDelete, IHasCreationTime, IHasDeletionTime, IHasModificationTime
    {
        public bool IsDeleted { get; private set; }
        public DateTimeOffset CreationTime { get; private set; } = DateTimeOffset.Now;
        public DateTimeOffset? DeletionTime { get; private set; }
        public DateTimeOffset? LastModificationTime { get; private set; }

        public virtual void SoftDelete()
        {
            IsDeleted = true;
            DeletionTime = DateTimeOffset.Now;
        }

        public void NotifyModified()
        {
            LastModificationTime = DateTimeOffset.Now;
        }
    }
}
