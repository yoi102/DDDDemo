using MediatR;

namespace Showcase.Domain.Events
{
    public record GameDeletedEvent(Guid Id) : INotification;
}
