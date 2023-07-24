using Microsoft.Extensions.DependencyInjection;
using Showcase.Domain;
using Zack.Commons;

namespace Showcase.Infrastructure
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IShowcaseRepository, ShowcaseRepository>();
        }
    }
}
