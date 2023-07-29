using Microsoft.Extensions.DependencyInjection;
using Zack.Commons;

namespace Showcase.Domain
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<ShowcaseDomainService>();
        }
    }
}