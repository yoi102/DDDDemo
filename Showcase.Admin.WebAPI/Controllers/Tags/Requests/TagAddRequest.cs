using Showcase.Domain.Entities;

namespace Showcase.Admin.WebAPI.Controllers.Tags.Requests
{
    public record TagAddRequest(GameId GameId, string Text);
}
