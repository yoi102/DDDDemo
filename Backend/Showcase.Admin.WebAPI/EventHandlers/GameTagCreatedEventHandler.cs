using Commons;
using MediatR;
using Showcase.Domain;
using Showcase.Domain.Events;
using Zack.EventBus;

namespace Showcase.Admin.WebAPI.EventHandlers
{
    public class GameTagCreatedEventHandler : INotificationHandler<GameTagCreatedEvent>
    {
        private readonly IEventBus eventBus;
        private readonly IShowcaseRepository repository;

        public GameTagCreatedEventHandler(IEventBus eventBus, IShowcaseRepository repository)
        {
            this.eventBus = eventBus;
            this.repository = repository;
        }

        public async Task Handle(GameTagCreatedEvent notification, CancellationToken cancellationToken)
        {
            //把领域事件转发为集成事件(RabbitMQ)，通知他微服务
            //发布集成事件，实现搜索索引、记录日志等功能
            //也可 SignalR 通知前端刷新
            var game = await repository.GetGameByIdAsync(notification.GameTag.GameId);
            var tags = await repository.GetTagsByGameIdAsync(notification.GameTag.GameId);
            var tagsStrings = tags.Select(x => x.Text).ToArray();

            eventBus.Publish(EventName.ShowcaseGameUpdated, new { game!.Id, game.Title, game.CoverUrl, game.Introduction, game.ReleaseDate, tagsStrings });
        }
    }
}