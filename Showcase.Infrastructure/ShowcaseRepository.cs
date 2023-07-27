using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Showcase.Domain;
using Showcase.Domain.Entities;

namespace Showcase.Infrastructure
{
    public class ShowcaseRepository : IShowcaseRepository
    {
        private readonly ShowcaseDbContext dbContext;

        public ShowcaseRepository(ShowcaseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Company[]> GetCompaniesAsync()
        {
            return await dbContext.Companies.OrderBy(c => c.SequenceNumber).ToArrayAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(CompanyId companyId)
        {
            return await dbContext.FindAsync<Company>(companyId);
        }

        public async Task<Exhibit?> GetExhibitByIdAsync(ExhibitId exhibitId)
        {
            return await dbContext.FindAsync<Exhibit>(exhibitId);
        }

        public async Task<Exhibit[]> GetExhibitsByGameIdAsync(GameId gameId)
        {
            return await dbContext.Exhibits.Where(e => e.GameId == gameId).OrderBy(e => e.SequenceNumber).ToArrayAsync();
        }

        public async Task<Game?> GetGameByIdAsync(GameId gameId)
        {
            return await dbContext.FindAsync<Game>(gameId);
        }

        public async Task<Game[]> GetGamesByCompanyIdAsync(CompanyId companyId)
        {
            return await dbContext.Games.Where(g => g.CompanyId == companyId).OrderBy(g => g.SequenceNumber).ToArrayAsync();
        }



        public async Task<int> GetMaxSequenceNumberOfCompaniesAsync()
        {
            int? maxSeq = await dbContext.Query<Company>().MaxAsync(c => (int?)c.SequenceNumber);
            return maxSeq ?? 0;
        }

        public async Task<int> GetMaxSequenceNumberOfExhibitsAsync(GameId gameId)
        {
            int? maxSeq = await dbContext.Query<Exhibit>().Where(e => e.GameId == gameId).MaxAsync(c => (int?)c.SequenceNumber);
            return maxSeq ?? 0;
        }

        public async Task<int> GetMaxSequenceNumberOfGamesAsync(CompanyId companyId)
        {
            int? maxSeq = await dbContext.Query<Game>().Where(g => g.CompanyId == companyId).MaxAsync(c => (int?)c.SequenceNumber);
            return maxSeq ?? 0;
        }

        public async Task<int> GetMaxSequenceNumberOfTagsAsync(GameId gameId)
        {
            int? maxSeq = await dbContext.Query<Tag>().Where(t => t.GameId == gameId).MaxAsync(c => (int?)c.SequenceNumber);
            return maxSeq ?? 0;
        }

        public async Task<Tag?> GetTagByIdAsync(TagId tagId)
        {
            return await dbContext.FindAsync<Tag>(tagId);
        }

        public async Task<Tag[]> GetTagsByGameIdAsync(GameId gameId)
        {
            return await dbContext.Tags.Where(g => g.GameId == gameId).OrderBy(t => t.SequenceNumber).ToArrayAsync();

        }



    }
}
