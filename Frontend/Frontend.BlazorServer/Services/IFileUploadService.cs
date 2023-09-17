namespace Frontend.BlazorServer.Services
{
    public interface IFileUploadService
    {
        Task<HttpResponseMessage> UploadFiles(HttpContent httpContent);







    }
}
