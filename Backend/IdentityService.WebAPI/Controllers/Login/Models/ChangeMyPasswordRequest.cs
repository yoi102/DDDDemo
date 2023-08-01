namespace IdentityService.WebAPI.Controllers.Login.Models
{
    public record ChangeMyPasswordRequest(string Password, string Password2);
}