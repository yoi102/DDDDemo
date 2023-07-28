using DomainCommons;

namespace Showcase.Admin.WebAPI.Controllers.Games.Requests
{
    public record GameUpdateRequest(MultilingualString Title, string Introduction, Uri CoverUrl, DateTimeOffset ReleaseDate);

}
