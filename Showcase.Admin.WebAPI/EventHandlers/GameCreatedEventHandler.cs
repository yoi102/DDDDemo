using Commons;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Showcase.Domain.Events;
using Showcase.Infrastructure;
using Zack.EventBus;

namespace Showcase.Admin.WebAPI.EventHandlers;

public class GameCreatedEventHandler : INotificationHandler<GameCreatedEvent>
{
    private readonly IEventBus eventBus;
    private readonly ShowcaseDbContext dbContext;

    public GameCreatedEventHandler(IEventBus eventBus,ShowcaseDbContext dbContext)
    {
        this.eventBus = eventBus;
        this.dbContext = dbContext;
    }

    public Task Handle(GameCreatedEvent notification, CancellationToken cancellationToken)
    {
        //把领域事件转发为集成事件(RabbitMQ)，通知他微服务
        //发布集成事件，实现搜索索引、记录日志等功能
        //也可 SignalR 通知前端刷新
        var game = notification.Value;
        var tags = dbContext.Tags.Where(x => game.TagIds.Contains(x.Id)).Select(x => x.Text).ToArray();
        eventBus.Publish(EventName.ShowcaseGameCreated, new { game.Id, game.Title, game.CoverUrl,game.Introduction, game.ReleaseDate, tags }); 
        return Task.CompletedTask;
    }
}