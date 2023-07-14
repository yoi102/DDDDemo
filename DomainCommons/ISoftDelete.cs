namespace DomainCommons
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; }
        void SoftDelete();
    }
}
