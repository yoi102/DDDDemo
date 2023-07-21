using Microsoft.AspNetCore.Identity;

namespace GameManagement.Api.DataAccess
{
    public class ApiIdentityUser : IdentityUser<long>
    {
        public DateTime CreationTime { get; set; }
        public long JWTVersion { get; set; }



    }
}