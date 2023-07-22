namespace FileService.Infrastructure.Services;
public class SMBStorageOptions
{
    /// <summary>
    /// 根目录
    /// </summary>
    public required string WorkingDirectory { get; set; }
}