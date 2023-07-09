using Microsoft.AspNetCore.Authorization;

namespace GameManagement.Api.Authorization
{
    public class AdministratorsHandler : AuthorizationHandler<QualifiedUserRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            QualifiedUserRequirement requirement)
        {
            if (context.User.IsInRole("Administrator"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}