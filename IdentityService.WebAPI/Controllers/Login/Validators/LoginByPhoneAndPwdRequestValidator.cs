using FluentValidation;
using IdentityService.WebAPI.Controllers.Login.Models;

namespace IdentityService.WebAPI.Controllers.Login.Validators
{
    public class LoginByPhoneAndPwdRequestValidator : AbstractValidator<LoginByPhoneAndPwdRequest>
    {
        public LoginByPhoneAndPwdRequestValidator()
        {
            RuleFor(e => e.PhoneNumber).NotNull().NotEmpty();
            RuleFor(e => e.Password).NotNull().NotEmpty();
        }
    }
}