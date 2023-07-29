using FileService.Domain;
using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure
{
    internal class FileServiceRepository : IFileServiceRepository
    {
        private readonly FileServiceDbContext dbContext;

        public FileServiceRepository(FileServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<UploadedItem?> FindFileAsync(long fileSize, string sha256Hash)
        {
            return dbContext.UploadItems.FirstOrDefaultAsync(u => u.FileSizeInBytes == fileSize
            && u.FileSHA256Hash == sha256Hash);
        }
    }
}