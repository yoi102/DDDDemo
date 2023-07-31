namespace SearchService.Domain
{
    public interface ISearchRepository
    {
        public Task UpsertAsync(Game game);
        public Task DeleteAsync(Guid gameId);
        public Task<SearchGamesResponse> SearchGames(string keyWord, int pageIndex, int pageSize);
    }
}