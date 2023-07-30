namespace SearchService.WebAPI.Controllers.Requests
{
    public record SearchEpisodesRequest(string Keyword, int PageIndex, int PageSize);

}
