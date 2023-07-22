namespace FileService.WebAPI.Controllers.Uploader.Models;


public record FileExistsResponse(bool IsExists, Uri? Url);