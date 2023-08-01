using FluentValidation;
using Showcase.Admin.WebAPI.Controllers.Exhibits.Request;

namespace Showcase.Admin.WebAPI.Controllers.Exhibits.Validators
{
    public class ExhibitUpdateRequestValidator : AbstractValidator<ExhibitUpdateRequest>
    {
        public ExhibitUpdateRequestValidator()
        {
            RuleFor(x => x.ItemUrl).NotEmpty().Length(5, 500);

        }
    }
}