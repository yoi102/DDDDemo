using FluentValidation;
using IdentityService.WebAPI.Controllers.Login.Models;

namespace IdentityService.WebAPI.Controllers.Login.Validators
{
    public class SendCodeByPhoneRequestValidator : AbstractValidator<SendCodeByPhoneRequest>
    {
        public SendCodeByPhoneRequestValidator()
        {
            RuleFor(e => e.PhoneNumber).NotNull().NotEmpty();
        }
    }
}