using MediatR;
using Showcase.Domain.Entities;

namespace Showcase.Domain.Events
{
    public record GameTagCreatedEvent(GameTag GameTag) : INotification;
}