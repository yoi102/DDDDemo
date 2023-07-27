using DomainCommons;

namespace Showcase.Domain.Entities
{
    public class GameTag : IEntity
    {
      
        public GameId GameId { get; set; }
        public TagId TagId { get; set; }

    }
}
