using Showcase.Domain.Entities;

namespace Showcase.Domain
{
    public interface IShowcaseRepository
    {
        Task<Company?> GetCompanyByIdAsync(CompanyId companyId);

        Task<Company[]> GetCompaniesAsync();

        Task<int> GetMaxSequenceNumberOfCompaniesAsync();

        Task<Game?> GetGameByIdAsync(GameId gameId);

        Task<Game[]> GetGamesByCompanyIdAsync(CompanyId companyId);

        Task<int> GetMaxSequenceNumberOfGamesAsync(CompanyId companyId);

        Task<Exhibit?> GetExhibitByIdAsync(ExhibitId exhibitId);

        Task<Exhibit[]> GetExhibitsByGameIdAsync(GameId gameId);

        Task<int> GetMaxSequenceNumberOfExhibitsAsync(GameId gameId);

        Task<Tag?> GetTagByIdAsync(TagId tagId);

        Task<Tag[]> GetTagsByGameIdAsync(GameId gameId);

        Task<Tag?> GetTagByTextAsync(string text);

        Task<GameTag?> GetGameTagByIdAsync(GameId gameId, TagId tagId);

        Task AddGameTagByIdAsync(GameId gameId, TagId tagId);
    }
}