namespace IdentityService.WebAPI.Controllers.Login.Models
{
    public record LoginByUserNameAndPwdRequest(string UserName, string Password);
}