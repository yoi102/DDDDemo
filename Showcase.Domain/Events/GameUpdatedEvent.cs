using MediatR;
using Showcase.Domain.Entities;

namespace Showcase.Domain.Events
{
    public record GameUpdatedEvent(Game Value) : INotification;
}
