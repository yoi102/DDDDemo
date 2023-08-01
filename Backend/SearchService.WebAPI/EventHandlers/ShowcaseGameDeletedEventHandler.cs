using Commons;
using SearchService.Domain;
using Zack.EventBus;

namespace SearchService.WebAPI.EventHandlers;

[EventName(EventName.ShowcaseGameDeleted)]
public class ShowcaseGameDeletedEventHandler : DynamicIntegrationEventHandler
{
    private readonly ISearchRepository repository;

    public ShowcaseGameDeletedEventHandler(ISearchRepository repository)
    {
        this.repository = repository;
    }

    public override Task HandleDynamic(string eventName, dynamic eventData)
    {
        Guid id = Guid.Parse(eventData.Id);
        return repository.DeleteAsync(id);
    }
}