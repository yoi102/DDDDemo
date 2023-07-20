using FluentValidation;
using IdentityService.WebAPI.Controllers.Login.Models;

namespace IdentityService.WebAPI.Controllers.Login.Validators
{
    public class LoginByUserNameAndPwdRequestValidator : AbstractValidator<LoginByUserNameAndPwdRequest>
    {
        public LoginByUserNameAndPwdRequestValidator()
        {
            RuleFor(e => e.UserName).NotNull().NotEmpty();
            RuleFor(e => e.Password).NotNull().NotEmpty();
        }
    }
}