using DomainCommons;
using Strongly;

namespace Showcase.Domain.Entities
{
    public record Exhibit : AggregateRootEntity, IAggregateRoot
    {
        public Exhibit(GameId gameId, ExhibitId id, Uri itemUrl, int sequenceNumber)
        {
            GameId = gameId;
            Id = id;
            ItemUrl = itemUrl;
            SequenceNumber = sequenceNumber;
        }
        public ExhibitId Id { get; private set; }
        public GameId GameId { get; private set; }
        public Uri ItemUrl { get; private set; }
        public int SequenceNumber { get; private set; }

        public Exhibit ChangeItemUrl(Uri value)
        {
            ItemUrl = value;
            return this;
        }
        public Exhibit ChangeSequenceNumber(int value)
        {
            SequenceNumber = value;
            return this;
        }
    }

    [Strongly(converters: StronglyConverter.EfValueConverter | StronglyConverter.SwaggerSchemaFilter | StronglyConverter.SystemTextJson)]
    public partial struct ExhibitId
    { }
}