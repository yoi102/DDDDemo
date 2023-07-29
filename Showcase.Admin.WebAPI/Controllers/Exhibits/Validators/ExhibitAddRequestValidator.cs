using FluentValidation;
using Showcase.Admin.WebAPI.Controllers.Exhibits.Request;

namespace Showcase.Admin.WebAPI.Controllers.Exhibits.Validators
{
    public class ExhibitAddRequestValidator : AbstractValidator<ExhibitAddRequest>
    {
        public ExhibitAddRequestValidator()
        {
            RuleFor(x => x.ItemUrl).Length(5, 500);//CoverUrl允许为空
        }
    }
}