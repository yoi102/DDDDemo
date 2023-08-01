using FluentValidation;
using IdentityService.WebAPI.Controllers.Login.Models;

namespace IdentityService.WebAPI.Controllers.Login.Validators
{
    public class LoginByPhoneAndCodeRequestValidator : AbstractValidator<LoginByPhoneAndCodeRequest>
    {
        public LoginByPhoneAndCodeRequestValidator()
        {
            RuleFor(e => e.PhoneNumber).NotNull().NotEmpty();
            RuleFor(e => e.Code).NotNull().NotEmpty();
        }
    }
}