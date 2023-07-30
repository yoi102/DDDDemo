using Commons;
using DomainCommons;
using SearchService.Domain;
using Zack.EventBus;

namespace SearchService.WebAPI.EventHandlers;

[EventName(EventName.ShowcaseGameCreated)]
[EventName(EventName.ShowcaseGameUpdated)]
public class ShowcaseGameUpsertEventHandler : DynamicIntegrationEventHandler
{
    private readonly ISearchRepository repository;

    public ShowcaseGameUpsertEventHandler(ISearchRepository repository)
    {
        this.repository = repository;
    }

    public override Task HandleDynamic(string eventName, dynamic eventData)
    {
        Guid id = Guid.Parse(eventData.Id);
        MultilingualString title = eventData.Title;
        Uri coverUrl = eventData.CoverUrl;
        string introduction = eventData.Introduction;
        Guid[] TagIds = eventData.TagIds;//..............

        Game game = new Game(id, title, coverUrl, introduction, TagIds);
        return repository.UpsertAsync(game);
    }
}
