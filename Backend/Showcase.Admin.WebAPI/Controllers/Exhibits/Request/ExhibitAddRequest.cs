using Showcase.Domain.Entities;

namespace Showcase.Admin.WebAPI.Controllers.Exhibits.Request
{
    public record ExhibitAddRequest(GameId GameId, Uri ItemUrl);
}