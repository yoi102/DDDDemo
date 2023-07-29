using FluentValidation;
using IdentityService.WebAPI.Controllers.UserAdmin.Models;

namespace IdentityService.WebAPI.Controllers.UserAdmin.Validators
{
    public class AddAdminUserRequestValidator : AbstractValidator<AddAdminUserRequest>
    {
        public AddAdminUserRequestValidator()
        {
            RuleFor(e => e.PhoneNumber).NotNull().NotEmpty().MaximumLength(11);
            RuleFor(e => e.UserName).NotEmpty().NotEmpty().MaximumLength(20).MinimumLength(2);
        }
    }
}