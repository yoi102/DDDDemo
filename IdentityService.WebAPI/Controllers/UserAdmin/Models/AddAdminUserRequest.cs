using FluentValidation;

namespace IdentityService.WebAPI.Controllers.UserAdmin.Models
{
    public record AddAdminUserRequest(string UserName, string PhoneNum);
}