using FluentValidation;
using IdentityService.WebAPI.Controllers.Login.Models;

namespace IdentityService.WebAPI.Controllers.Login.Validators
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangeMyPasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(e => e.Password).NotNull().NotEmpty()
                .Equal(e => e.Password2);
            RuleFor(e => e.Password2).NotNull().NotEmpty();
        }
    }
}