using DomainCommons;

namespace Showcase.Admin.WebAPI.Controllers.Companies.Requests
{
    public record CompanyAddRequest(string Name, Uri CoverUrl);
   
}
