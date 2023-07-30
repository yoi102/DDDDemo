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
          
            return Task.CompletedTask;
        }
    }
}
