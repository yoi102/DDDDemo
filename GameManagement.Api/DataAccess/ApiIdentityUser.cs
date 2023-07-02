using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Api.DataAccess
{
    public class ApiIdentityUser : IdentityUser
    {
        public ApiIdentityUser()
        {
            Claims = new List<IdentityUserClaim<string>>();
            Logins = new List<IdentityUserLogin<string>>();
            Tokens = new List<IdentityUserToken<string>>();
            UserRoles = new List<IdentityUserRole<string>>();
        }

        public ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public ICollection<IdentityUserToken<string>> Tokens { get; set; }
        public ICollection<IdentityUserRole<string>> UserRoles { get; set; }



        [MaxLength(18)]
        public string? IdCardNo { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
