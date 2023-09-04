using System.ComponentModel.DataAnnotations;

namespace Frontend.BlazorServer.Authentication
{
    public class LoginByUserNameAndPwdModel
    {
        [Required(AllowEmptyStrings = false)]
        public string? UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? Password { get; set; }

    }
}
