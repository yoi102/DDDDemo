
using Commons;
using MediatR;
using Showcase.Domain.Events;
using Zack.EventBus;

namespace Listening.Admin.WebAPI.EventHandlers;
public class EpisodeUpdatedEventHandler : INotificationHandler<GameUpdatedEvent>
{
    private readonly IEventBus eventBus;

    public EpisodeUpdatedEventHandler(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public Task Handle(GameUpdatedEvent notification, CancellationToken cancellationToken)
    {

        var game = notification.Value;
        eventBus.Publish(EventName.ShowcaseGameDeleted, new { game.Id, game.Title, game.CoverUrl, game.Introduction, game.ReleaseDate });

        return Task.CompletedTask;
    }
}
