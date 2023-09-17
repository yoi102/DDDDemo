using FileService.Domain.Entities;
using Zack.Commons;

namespace FileService.Domain;

public class FileServiceDomainService
{
    private readonly IFileServiceRepository repository;
    private readonly IStorageClient backupStorage;//备份服务器
    private readonly IStorageClient remoteStorage;//文件存储服务器

    public FileServiceDomainService(IFileServiceRepository repository,
        IEnumerable<IStorageClient> storageClients)
    {
        this.repository = repository;
        //解决内置DI不能使用名字注入不同实例的问题
        this.backupStorage = storageClients.First(c => c.StorageType == StorageType.Backup);
        this.remoteStorage = storageClients.First(c => c.StorageType == StorageType.Public);
    }

    //领域服务只有抽象的业务逻辑
    public async Task<UploadedItemResult> UploadAsync(Stream stream, string fileName, CancellationToken cancellationToken)
    {
        string hash = HashHelper.ComputeSha256Hash(stream);
        long fileSize = stream.Length;
        DateTime today = DateTime.Today;
        //用日期把文件分散在不同文件夹存储，同时由于加上了文件hash值作为目录，用户上传的文件夹做文件名，
        //所以几乎不会发生不同文件冲突的可能
        //用用户上传的文件名保存文件名，这样用户查看、下载文件的时候，文件名更灵活
        string partialPath = $"{today.Year}/{today.Month}/{today.Day}/{hash}/{fileName}";

        //查询是否有和上传文件的大小和SHA256一样的文件，如果有的话，就认为是同一个文件
        //虽然说前端可能已经调用FileExists接口检查过了，但是前端可能跳过了，或者有并发上传等问题，所以这里再检查一遍。
        var oldUploadItem = await repository.FindFileAsync(fileSize, hash);
        if (oldUploadItem is not null)
        {
            return new UploadedItemResult(true, oldUploadItem);
        }
        stream.Position = 0;
        Uri backupUrl = await backupStorage.SaveAsync(partialPath, stream, cancellationToken);//保存到本地备份
        stream.Position = 0;
        Uri remoteUrl = await remoteStorage.SaveAsync(partialPath, stream, cancellationToken);//保存到生产的存储系统
        stream.Position = 0;
        Guid id = Guid.NewGuid();
        return new UploadedItemResult(true, new UploadedItem(id, fileSize, fileName, hash, backupUrl, remoteUrl));

    }
}