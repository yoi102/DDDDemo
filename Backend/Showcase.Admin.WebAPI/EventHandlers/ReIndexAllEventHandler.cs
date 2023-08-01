using Commons;
using Infrastructure.EFCore;
using Showcase.Domain;
using Showcase.Domain.Entities;
using Showcase.Infrastructure;
using Zack.EventBus;

namespace Listening.Admin.WebAPI.EventHandlers
{
    [EventName(EventName.SearchServiceReIndexAll)]
    public class ReIndexAllEventHandler : IIntegrationEventHandler
    {
        private readonly ShowcaseDbContext dbContext;
        private readonly IShowcaseRepository repository;
        private readonly IEventBus eventBus;

        public ReIndexAllEventHandler(ShowcaseDbContext dbContext, IShowcaseRepository repository, IEventBus eventBus)
        {
            this.dbContext = dbContext;
            this.repository = repository;
            this.eventBus = eventBus;
        }

        public async Task Handle(string eventName, string eventData)
        {
            List<Game> games = new List<Game>();

            foreach (var game in dbContext.Query<Game>())
            {
                games.Add(game);
            }
            //防止.......
            //System.InvalidOperationException:“There is already an open DataReader associated with this Connection which must be closed first.”
            foreach (Game game in games)
            {
                if (game.IsDeleted)
                    continue;

                var tags = await repository.GetTagsByGameIdAsync(game.Id);
                var tagsStrings = tags.Select(x => x.Text).ToArray();
                eventBus.Publish(EventName.ShowcaseGameUpdated, new { game.Id, game.Title, game.CoverUrl, game.Introduction, game.ReleaseDate, tagsStrings });
            }
        }
    }
}