
using Commons;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Showcase.Domain;
using Showcase.Domain.Events;
using Showcase.Infrastructure;
using Zack.EventBus;

namespace Listening.Admin.WebAPI.EventHandlers;
public class GameUpdatedEventHandler : INotificationHandler<GameUpdatedEvent>
{
    private readonly IEventBus eventBus;
    private readonly ShowcaseDbContext dbContext;

    public GameUpdatedEventHandler(IEventBus eventBus, ShowcaseDbContext dbContext)
    {
        this.eventBus = eventBus;
        this.dbContext = dbContext;
    }

    public Task Handle(GameUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var game = notification.Value;
        var tags = dbContext.Tags.Where(x => game.TagIds.Contains(x.Id)).Select(x => x.Text).ToArray();
        eventBus.Publish(EventName.ShowcaseGameUpdated, new { game.Id, game.Title, game.CoverUrl, game.Introduction, game.ReleaseDate, tags });
        return Task.CompletedTask;
    }
}
