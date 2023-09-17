
namespace Frontend.BlazorServer.Services
{
    public class FileUploadService: IFileUploadService
    {
        private readonly HttpClient http;

        //public FileUploadService(IHttpClientFactory httpClientFactory )
        //{
        //  http=  httpClientFactory.CreateClient("nginx-server");
        //}
        public FileUploadService( HttpClient http)
        {
            this.http = http;
        }

        public async Task<HttpResponseMessage> UploadFiles(HttpContent httpContent)
        {
            //HttpClient http = new HttpClient();
            //return await http.PostAsync("http://localhost:7071/api/uploader/flies", httpContent);
            return await http.PostAsync("FileService/api/uploader/flies", httpContent);
        }              








    }
}
