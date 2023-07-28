using FluentValidation;
using Showcase.Admin.WebAPI.Controllers.Companies.Requests;

namespace Showcase.Admin.WebAPI.Controllers.Companies.Validators
{
    public class CompanyUpdateRequestValidator : AbstractValidator<CompanyUpdateRequest>
    {
        public CompanyUpdateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.CoverUrl).Length(5, 500);
        }
    }
}