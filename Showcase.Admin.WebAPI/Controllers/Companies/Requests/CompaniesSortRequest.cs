using Showcase.Domain.Entities;

namespace Showcase.Admin.WebAPI.Controllers.Companies.Requests
{
    public class CompaniesSortRequest
    {
        public required CompanyId[] SortedCompanyIds { get; set; }

    }
}
