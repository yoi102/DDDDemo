using FileService.Domain;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.Services
{
    /// <summary>
    /// 用局域网内共享文件夹或者本机磁盘当备份服务器的实现类
    /// </summary>
    internal class SMBStorageClient : IStorageClient
    {
        private readonly IOptionsSnapshot<SMBStorageOptions> options;

        public SMBStorageClient(IOptionsSnapshot<SMBStorageOptions> options)
        {
            this.options = options;
        }

        public StorageType StorageType => StorageType.Backup;

        public async Task<Uri> SaveAsync(string partialPath, Stream content, CancellationToken cancellationToken = default)
        {
            if (partialPath.StartsWith('/'))
            {
                throw new ArgumentException("partialPath should not start with /", nameof(partialPath));
            }
            string workingDir = options.Value.WorkingDirectory;
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
            return new Uri(fullPath);
        }
    }
}
