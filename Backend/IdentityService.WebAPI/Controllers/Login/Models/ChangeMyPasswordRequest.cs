namespace IdentityService.WebAPI.Controllers.Login.Models
{
    public record ChangeMyPasswordRequest(string NewPassword, string OldPassword);
}