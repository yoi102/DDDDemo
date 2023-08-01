using Commons;
using MediatR;
using Showcase.Domain;
using Showcase.Domain.Events;
using Showcase.Infrastructure;
using Zack.EventBus;

namespace Showcase.Admin.WebAPI.EventHandlers;

public class GameCreatedEventHandler : INotificationHandler<GameCreatedEvent>
{
    private readonly IEventBus eventBus;
    private readonly IShowcaseRepository repository;

    public GameCreatedEventHandler(IEventBus eventBus, ShowcaseDbContext dbContext, IShowcaseRepository repository)
    {
        this.eventBus = eventBus;
        this.repository = repository;
    }

    public async Task Handle(GameCreatedEvent notification, CancellationToken cancellationToken)
    {
        //把领域事件转发为集成事件(RabbitMQ)，通知他微服务
        //发布集成事件，实现搜索索引、记录日志等功能
        //也可 SignalR 通知前端刷新
        var game = notification.Value;
        var tags = await repository.GetTagsByGameIdAsync(game.Id);
        var tagsStrings = tags.Select(x => x.Text).ToArray();
        eventBus.Publish(EventName.ShowcaseGameCreated, new { game.Id, game.Title, game.CoverUrl, game.Introduction, game.ReleaseDate, tagsStrings });
    }
}