namespace IdentityService.WebAPI.Controllers.Login.Models
{
    public record LoginByPhoneAndPwdRequest(string PhoneNum, string Password);

}
