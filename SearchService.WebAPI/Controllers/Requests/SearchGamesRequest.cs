namespace SearchService.WebAPI.Controllers.Requests
{
    public record SearchGamesRequest(string Keyword, int PageIndex, int PageSize);

}
