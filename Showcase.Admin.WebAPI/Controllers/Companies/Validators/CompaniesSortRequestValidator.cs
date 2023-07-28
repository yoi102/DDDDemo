using FluentValidation;
using Showcase.Admin.WebAPI.Controllers.Companies.Requests;

namespace Showcase.Admin.WebAPI.Controllers.Companies.Validators
{
    public class CompaniesSortRequestValidator : AbstractValidator<CompaniesSortRequest>
    {
        public CompaniesSortRequestValidator()
        {
            RuleFor(r => r.SortedCompanyIds).NotNull().NotEmpty().NotDuplicated().NotContains(new Domain.Entities.CompanyId( Guid.Empty));
            //RuleFor(r => r.SortedCompanyIds).NotNull().NotEmpty().NotContains(Guid.Empty).NotDuplicated();
        }
    }
}