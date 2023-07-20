namespace IdentityService.WebAPI.Controllers.Login.Models
{
    public record LoginByPhoneAndCodeRequest(string PhoneNum, string Code);

}
