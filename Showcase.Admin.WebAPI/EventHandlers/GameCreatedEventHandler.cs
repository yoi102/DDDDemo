using Commons;
using MediatR;
using Showcase.Domain.Events;
using Zack.EventBus;

namespace Showcase.Admin.WebAPI.EventHandlers;

public class GameCreatedEventHandler : INotificationHandler<GameCreatedEvent>
{
    private readonly IEventBus eventBus;

    public GameCreatedEventHandler(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public Task Handle(GameCreatedEvent notification, CancellationToken cancellationToken)
    {
        //把领域事件转发为集成事件(RabbitMQ)，通知他微服务
        //发布集成事件，实现搜索索引、记录日志等功能
        //也可 SignalR 通知前端刷新
        var game = notification.Value;
        eventBus.Publish(EventName.ShowcaseGameCreated, new { game.Id, game.Title, game.CoverUrl, game.Introduction, game.ReleaseDate });
        return Task.CompletedTask;
    }
}