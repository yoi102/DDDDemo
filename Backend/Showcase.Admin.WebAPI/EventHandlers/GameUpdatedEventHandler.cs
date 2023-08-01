using Commons;
using MediatR;
using Showcase.Domain;
using Showcase.Domain.Events;
using Zack.EventBus;

namespace Listening.Admin.WebAPI.EventHandlers;

public class GameUpdatedEventHandler : INotificationHandler<GameUpdatedEvent>
{
    private readonly IEventBus eventBus;
    private readonly IShowcaseRepository repository;

    public GameUpdatedEventHandler(IEventBus eventBus, IShowcaseRepository repository)
    {
        this.eventBus = eventBus;
        this.repository = repository;
    }

    public async Task Handle(GameUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var game = notification.Value;
        var tags = await repository.GetTagsByGameIdAsync(game.Id);
        var tagsStrings = tags.Select(x => x.Text).ToArray();
        eventBus.Publish(EventName.ShowcaseGameUpdated, new { game.Id, game.Title, game.CoverUrl, game.Introduction, game.ReleaseDate, tagsStrings });
    }
}