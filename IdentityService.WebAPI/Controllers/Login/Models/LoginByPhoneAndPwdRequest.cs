namespace IdentityService.WebAPI.Controllers.Login.Models
{
    public record LoginByPhoneAndPwdRequest(string PhoneNumber, string Password);

}
