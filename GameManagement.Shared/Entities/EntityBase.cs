namespace GameManagement.Shared.Entities
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}