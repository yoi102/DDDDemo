using Showcase.Domain.Entities;

namespace Showcase.Admin.WebAPI.Controllers.Exhibits.Request
{
    public record ExhibitUpdateRequest(GameId GameId, Uri ItemUrl);
}