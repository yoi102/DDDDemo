using DomainCommons;
using Showcase.Domain.Entities;

namespace Showcase.Admin.WebAPI.Controllers.Games.Requests
{
    public record GameAddRequest(CompanyId CompanyId, MultilingualString Title, string Introduction, Uri CoverUrl, DateTimeOffset ReleaseDate);
}