using FileService.Domain;
using FileService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Zack.Commons;

namespace FileService.Infrastructure
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IStorageClient, SMBStorageClient>();
            services.AddScoped<IStorageClient, MockCloudStorageClient>();
            services.AddScoped<IFileServiceRepository, FileServiceRepository>();
            services.AddScoped<FileServiceDomainService>();
            services.AddHttpClient();
        }
    }
}
