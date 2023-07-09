using Microsoft.AspNetCore.Authorization;

namespace GameManagement.Api.Authorization
{
    public class CanEditHandler : AuthorizationHandler<QualifiedUserRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            QualifiedUserRequirement requirement)
        {
            if (context.User.HasClaim(x => x.Type == "Manager"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}