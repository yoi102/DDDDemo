using FluentValidation;
using Showcase.Admin.WebAPI.Controllers.Exhibits.Request;

namespace Showcase.Admin.WebAPI.Controllers.Exhibits.Validators
{
    public class ExhibitsSortRequestValidator : AbstractValidator<ExhibitsSortRequest>
    {
        public ExhibitsSortRequestValidator()
        {
            RuleFor(r => r.SortedExhibitIds).NotNull().NotEmpty().NotContains(new Domain.Entities.ExhibitId(Guid.Empty)).NotDuplicated();

            //RuleFor(r => r.SortedExhibitIds).NotNull().NotEmpty().NotContains(Guid.Empty).NotDuplicated();
        }
    }
}