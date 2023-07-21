﻿using Microsoft.Extensions.DependencyInjection;
using Zack.Commons;

namespace ASPNETCore
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddScoped<IMemoryCacheHelper, MemoryCacheHelper>();
            services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();
        }
    }
}
