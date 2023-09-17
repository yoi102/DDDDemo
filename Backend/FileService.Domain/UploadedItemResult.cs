using FileService.Domain.Entities;

namespace FileService.Domain
{
    public record UploadedItemResult(bool isOldUploadedItem, UploadedItem UploadedItem);

}
