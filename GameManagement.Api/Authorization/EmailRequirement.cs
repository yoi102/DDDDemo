using Microsoft.AspNetCore.Authorization;

namespace GameManagement.Api.Authorization
{
    public class EmailRequirement : IAuthorizationRequirement
    {
        public string RequiredEmail { get; set; }

        public EmailRequirement(string requiredEmail)
        {
            RequiredEmail = requiredEmail;
        }
    }
}