using Commons;
using Microsoft.AspNetCore.Mvc;
using SearchService.Domain;
using SearchService.WebAPI.Controllers.Requests;
using Zack.EventBus;

namespace SearchService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/search")]
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
        public async Task<ActionResult<SearchGamesResponse?>> SearchGames([FromQuery] SearchGamesRequest request)
        {
            var response = await repository.SearchGames(request.Keyword, request.PageIndex, request.PageSize);
            if (response is null || response.TotalCount == 0)
            {
                return NotFound(request);
            }
            return response;
        }

        [HttpPut]
        public IActionResult ReIndexAll()
        {
            eventBus.Publish(EventName.SearchServiceReIndexAll, null);
            return Ok();
        }
    }
}