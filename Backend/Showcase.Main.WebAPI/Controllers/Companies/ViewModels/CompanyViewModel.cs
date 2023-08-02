using Showcase.Domain.Entities;

namespace Showcase.Main.WebAPI.Controllers.Companies.ViewModels
{
    public record CompanyViewModel(CompanyId Id, string name, Uri CoverUrl)
    {
        public static CompanyViewModel? Create(Company? company)
        {
            if (company == null)
            {
                return null;
            }
            return new CompanyViewModel(company.Id, company.Name, company.CoverUrl);
        }
        public static CompanyViewModel[] Create(Company[] companies)
        {
            return companies.Select(c => Create(c)!).ToArray();
        }
    }
}