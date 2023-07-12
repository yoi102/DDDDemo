using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Api.DataAccess
{
    public class ApiIdentityUser : IdentityUser<long>
    {
        public DateTime CreationTime { get; set; }
        public long JWTVersion { get; set; }



    }
}