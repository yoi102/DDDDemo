using IdentityService.Domain.Entities;

namespace IdentityService.WebAPI.Controllers.UserAdmin.Models;
public record UserDTO(Guid Id, string? UserName, string? PhoneNumber, DateTimeOffset CreationTime)
{
    public static UserDTO Create(User user)
    {
        return new UserDTO(user.Id, user.UserName, user.PhoneNumber, user.CreationTime);
    }
}