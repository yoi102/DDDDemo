using Showcase.Domain.Entities;

namespace Showcase.Admin.WebAPI.Controllers.Games.Requests
{
    public class GamesSortRequest
    {
        public required GameId[] SortedGameIds { get; set; }

    }
}
