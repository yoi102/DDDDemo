using FileService.WebAPI.Controllers.Uploader.Models;
using FluentValidation;

namespace FileService.WebAPI.Controllers.Uploader.Validators;

public class UploadRequestValidator : AbstractValidator<UploadRequest>
{
    public UploadRequestValidator()
    {
        //不用校验文件名的后缀，因为文件服务器会做好安全设置，即使用户上传exe、php等文件都是可以的
        long maxFileSize = 50 * 1024 * 1024;//最大文件大小
        RuleFor(e => e.File).NotNull().Must(f => f.Length > 0 && f.Length < maxFileSize);
    }
}