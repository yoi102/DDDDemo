using DomainCommons;
using Showcase.Domain.Events;

namespace Showcase.Domain.Entities
{
    public record GameTag : BaseEntity
    {
        private GameTag()
        {
        }
        public static GameTag Create(GameId gameId, TagId tagId)
        {
            var gameTag = new GameTag
            {
                GameId = gameId,
                TagId = tagId
            };
            gameTag.AddDomainEventIfAbsent(new GameTagCreatedEvent(gameTag));
            return gameTag;
        }

        public GameId GameId { get; private set; }
        public TagId TagId { get; private set; }
    }
}