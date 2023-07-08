using Microsoft.AspNetCore.Authorization;

namespace GameManagement.Api.Authorization
{
    public class EmailHandler : AuthorizationHandler<EmailRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EmailRequirement requirement)
        {
            var claim = context.User.Claims.FirstOrDefault(x => x.Type == "Email");

            if (claim != null)
            {
                if (claim.Value.EndsWith(requirement.RequiredEmail))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
