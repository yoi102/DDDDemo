using Showcase.Domain;
using Showcase.Domain.Entities;

namespace Showcase.Infrastructure
{
    public class ShowcaseRepository : IShowcaseRepository
    {
        public Task<Company[]> GetCompaniesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Company?> GetCompanyByIdAsync(CompanyId companyId)
        {
            throw new NotImplementedException();
        }

        public Task<Exhibit?> GetExhibitByIdAsync(ExhibitId exhibitId)
        {
            throw new NotImplementedException();
        }

        public Task<Exhibit[]> GetExhibitsByGameIdAsync(GameId gameId)
        {
            throw new NotImplementedException();
        }

        public Task<Game?> GetGameByIdAsync(GameId gameId)
        {
            throw new NotImplementedException();
        }

        public Task<Game[]> GetGamesByCompanyIdAsync(CompanyId companyId)
        {
            throw new NotImplementedException();
        }

        public Task<Game[]> GetGamesByTagIdAsync(TagId tagId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetMaxSequenceNumberOfCompaniesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetMaxSequenceNumberOfExhibitsAsync(GameId gameId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetMaxSequenceNumberOfGamesAsync(CompanyId companyId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetMaxSequenceNumberOfTagsAsync(GameId gameId)
        {
            throw new NotImplementedException();
        }

        public Task<Tag?> GetTagByIdAsync(TagId tagId)
        {
            throw new NotImplementedException();
        }

        public Task<Tag[]> GetTagsByGameIdAsync(GameId gameId)
        {
            throw new NotImplementedException();
        }
    }
}
