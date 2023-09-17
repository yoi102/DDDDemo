using FileService.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FileService.Infrastructure.Services
{
    /// <summary>
    /// 把FileService.WebAPI当成一个云存储服务器，是一个Mock。文件保存在wwwroot文件夹下。
    /// 仅供开发、演示阶段使用
    /// </summary>
    internal class MockCloudStorageClient : IStorageClient
    {
        public StorageType StorageType => StorageType.Public;

        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly NginxOptions nginxOptions;

        public MockCloudStorageClient(IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this.hostEnvironment = hostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            nginxOptions = configuration.GetSection("Nginx").Get<NginxOptions>()!;
        }

        public async Task<Uri> SaveAsync(string partialPath, Stream content, CancellationToken cancellationToken = default)
        {
            if (partialPath.StartsWith('/'))
            {
                throw new ArgumentException("partialPath should not start with /", nameof(partialPath));
            }
            string workingDir = Path.Combine(hostEnvironment.ContentRootPath, "wwwroot");
            string fullPath = Path.Combine(workingDir, partialPath);
            string? fullDir = Path.GetDirectoryName(fullPath);//get the directory
            if (!Directory.Exists(fullDir))//automatically create dir
            {
                Directory.CreateDirectory(fullDir!);
            }
            if (File.Exists(fullPath))//如果已经存在，则尝试删除
            {
                File.Delete(fullPath);
            }
            using Stream outStream = File.OpenWrite(fullPath);
            await content.CopyToAsync(outStream, cancellationToken);
            var req = httpContextAccessor.HttpContext!.Request;
            ////string url = req.Scheme + "://" + req.Host + "/" + partialPath;
            //string url = req.Scheme + "://" + req.Host + "/FileService/" + partialPath;
            //string url = req.Scheme + "://" + req.Host + ":8080/FileService/" + partialPath;//需要与 nginx 的匹配

            string url = nginxOptions.Scheme + "://" + nginxOptions.ServerName + ":" + nginxOptions.Listen + "/FileService/" + partialPath;
            return new Uri(url);
        }

        public class NginxOptions
        {
            public required string Listen { get; set; }
            public required string ServerName { get; set; }
            public required string Scheme { get; set; }
        }


    }


}