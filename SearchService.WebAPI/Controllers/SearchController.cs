using Commons;
using Microsoft.AspNetCore.Mvc;
using SearchService.Domain;
using SearchService.WebAPI.Controllers.Requests;
using Zack.EventBus;

namespace SearchService.WebAPI.Controllers
{
    [ApiController]
    [Route("search")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository repository;
        private readonly IEventBus eventBus;

        public SearchController(ISearchRepository repository, IEventBus eventBus)
        {
            this.repository = repository;
            this.eventBus = eventBus;
        }


        [HttpGet]
        public Task<SearchGamesResponse> SearchEpisodes([FromQuery] SearchEpisodesRequest request)
        {
            return repository.SearchEpisodes(request.Keyword, request.PageIndex, request.PageSize);
        }

        [HttpPut]
        public IActionResult ReIndexAll()
        {
            eventBus.Publish(EventName.SearchServiceReIndexAll, null);
            return Ok();
        }


    }














}
