
using MediatR;
using Showcase.Domain.Events;
using Zack.EventBus;

namespace Listening.Admin.WebAPI.EventHandlers;
public class GameCreatedEventHandler : INotificationHandler<GameCreatedEvent>
{
    private readonly IEventBus eventBus;

    public GameCreatedEventHandler(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public Task Handle(GameCreatedEvent notification, CancellationToken cancellationToken)
    {
        //把领域事件转发为集成事件(RabbitMQ)，让其他微服务听到
        //发布集成事件，实现搜索索引、记录日志等功能
        //也可 SignalR 通知前端刷新 
        var game = notification.Value;
        eventBus.Publish("Showcase.GameCreated", new { game.Id, game.Title, game.CoverUrl, game.Introduction, game.ReleaseDate });
        return Task.CompletedTask;
    }
}
