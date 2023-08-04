using FluentValidation;
using IdentityService.WebAPI.Controllers.Login.Models;

namespace IdentityService.WebAPI.Controllers.Login.Validators
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangeMyPasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(e => e.NewPassword).NotNull().NotEmpty()
                .Equal(e => e.OldPassword);
            RuleFor(e => e.OldPassword).NotNull().NotEmpty();
        }
    }
}