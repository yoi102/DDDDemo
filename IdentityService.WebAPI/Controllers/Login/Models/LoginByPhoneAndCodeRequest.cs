namespace IdentityService.WebAPI.Controllers.Login.Models
{
    public record LoginByPhoneAndCodeRequest(string PhoneNumber, string Code);

}
