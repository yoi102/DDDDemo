using IdentityService.Domain;
using IdentityService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Zack.Commons;

namespace IdentityService.Infrastructure
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IEmailSender, MockEmailSender>();
            services.AddScoped<ISmsSender, MockSmsSender>();
            services.AddScoped<IdentityDomainService>();
            services.AddScoped<IIdentityRepository, IdentityRepository>();
        }
    }
}