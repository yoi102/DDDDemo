using Microsoft.Extensions.DependencyInjection;
using Zack.Commons;

namespace JWT
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}