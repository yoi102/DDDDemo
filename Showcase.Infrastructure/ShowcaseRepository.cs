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
            return await dbContext.Exhibits.OrderBy(e => e.SequenceNumber).Where(e => e.GameId == gameId).ToArrayAsync();
        }

        public async Task<Game?> GetGameByIdAsync(GameId gameId)
        {
            return await dbContext.FindAsync<Game>(gameId);
        }

        public async Task<Game[]> GetGamesByCompanyIdAsync(CompanyId companyId)
        {
            return await dbContext.Games.OrderBy(g => g.SequenceNumber).Where(g => g.CompanyId == companyId).ToArrayAsync();
        }

        public Task<Game[]> GetGamesByTagIdAsync(TagId tagId)
        {
            var c = dbContext.GameTags.Where(x => x.Equals(tagId)).Select(x => x.GameId);
            return dbContext.Games.Where(x => c.Contains(x.Id)).OrderBy(g => g.SequenceNumber).ToArrayAsync();
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
            var c = dbContext.GameTags.Where(x => x.Equals(gameId)).Select(x => x.TagId);
            int? maxSeq = await dbContext.Tags.Where(x => c.Contains(x.Id)).OrderBy(g => g.SequenceNumber).MaxAsync(c => (int?)c.SequenceNumber);
            return maxSeq ?? 0;
        }

        public async Task<Tag?> GetTagByIdAsync(TagId tagId)
        {
            return await dbContext.FindAsync<Tag>(tagId);
        }

        public Task<Tag[]> GetTagsByGameIdAsync(GameId gameId)
        {
            var c = dbContext.GameTags.Where(x => x.Equals(gameId)).Select(x => x.TagId);
            return dbContext.Tags.Where(x => c.Contains(x.Id)).OrderBy(g => g.SequenceNumber).ToArrayAsync();
        }

        //public Task


    }
}
