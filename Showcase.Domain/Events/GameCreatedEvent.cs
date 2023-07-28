using MediatR;
using Showcase.Domain.Entities;

namespace Showcase.Domain.Events
{
    public record GameCreatedEvent(Game Game) : INotification;

}
