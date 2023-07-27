using DomainCommons;
using Strongly;

namespace Showcase.Domain.Entities
{
    public record Tag : BaseEntity
    {
        public Tag(GameId gameId, TagId id,  string text, int sequenceNumber)
        {
            GameId = gameId;
            Id = id;
            Text = text;
            SequenceNumber = sequenceNumber;
        }

        public TagId Id { get; private set; }
        public GameId GameId { get; private set; }

        public string Text { get; private set; }
        public int SequenceNumber { get; private set; }

        public Tag ChangeText(string value)
        {
            Text = value;
            return this;
        }
        public Tag ChangeSequenceNumber(int value)
        {
            SequenceNumber = value;
            return this;
        }

    }
    [Strongly]
    public partial struct TagId { }

}
