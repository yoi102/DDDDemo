namespace GameManagement.Shared.Entities
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdateDate { get; set; }


    }
}
