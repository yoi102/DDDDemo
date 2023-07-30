

using Commons;
using MediatR;
using Showcase.Domain.Events;
using Zack.EventBus;

namespace Listening.Admin.WebAPI.EventHandlers;
public class GameDeletedEventHandler : INotificationHandler<GameDeletedEvent>
{
    private readonly IEventBus eventBus;

    public GameDeletedEventHandler(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public Task Handle(GameDeletedEvent notification, CancellationToken cancellationToken)
    {
        var id = notification.Id;
        eventBus.Publish(EventName.ShowcaseGameDeleted, new { Id = id });
        return Task.CompletedTask;
    }
}
