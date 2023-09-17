
namespace Frontend.BlazorServer.Services
{
    public class FileUploadService: IFileUploadService
    {
        private readonly HttpClient http;

        public FileUploadService(IHttpClientFactory httpClientFactory )
        {
          http=  httpClientFactory.CreateClient("nginx-server");
        }


        public async Task<HttpResponseMessage> UploadFiles(HttpContent httpContent)
        {
            return await http.PostAsync("FileService/api/uploader/files", httpContent);
        }              








    }
}
