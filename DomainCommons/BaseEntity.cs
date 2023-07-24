using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainCommons
{
    public record BaseEntity : IEntity, IDomainEvents
    {
        [NotMapped]
        private readonly List<INotification> domainEvents = new();

        public void AddDomainEvent(INotification eventItem)
        {
            domainEvents.Add(eventItem);
        }

        public void AddDomainEventIfAbsent(INotification eventItem)
        {
            if (!domainEvents.Contains(eventItem))
            {
                domainEvents.Add(eventItem);
            }
        }
        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }

        public IEnumerable<INotification> GetDomainEvents()
        {
            return domainEvents;
        }
    }
}