using FluentValidation;
using IdentityService.WebAPI.Controllers.UserAdmin.Models;

namespace IdentityService.WebAPI.Controllers.UserAdmin.Validators
{
    public class EditAdminUserRequestValidator : AbstractValidator<EditAdminUserRequest>
    {
        public EditAdminUserRequestValidator()
        {
            RuleFor(e => e.PhoneNumber).NotNull().NotEmpty();
        }
    }
}