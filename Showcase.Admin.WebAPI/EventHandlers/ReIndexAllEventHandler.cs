using Showcase.Domain.Entities;
using Showcase.Infrastructure;
using Zack.EventBus;
using Infrastructure.EFCore;
using Commons;

namespace Listening.Admin.WebAPI.EventHandlers
{
    [EventName(EventName.SearchServiceReIndexAll)]
    public class ReIndexAllEventHandler : IIntegrationEventHandler
    {
        private readonly ShowcaseDbContext dbContext;
        private readonly IEventBus eventBus;

        public ReIndexAllEventHandler(ShowcaseDbContext dbContext, IEventBus eventBus)
        {
            this.dbContext = dbContext;
            this.eventBus = eventBus;
        }

        public Task Handle(string eventName, string eventData)
        {
            foreach (var game in dbContext.Query<Game>())
            {
                var tags = dbContext.Tags.Where(x => game.TagIds.Contains(x.Id)).Select(x => x.Text).ToArray();
                eventBus.Publish(EventName.ShowcaseGameUpdated, new { game.Id, game.Title, game.CoverUrl, game.Introduction, game.ReleaseDate, tags });
            }
            return Task.CompletedTask;
        }
    }
}
