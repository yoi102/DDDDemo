using DomainCommons;
using Showcase.Domain.Entities;

namespace Showcase.Domain
{
    public class ShowcaseDomainService
    {
        private readonly IShowcaseRepository repository;

        public ShowcaseDomainService(IShowcaseRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Company> AddCompanyAsync(string name, Uri coverUrl)
        {
            int maxSeq = await repository.GetMaxSequenceNumberOfCompaniesAsync();
            return new Company(new CompanyId(Guid.NewGuid()), name, coverUrl, maxSeq + 1);
        }

        public async Task SortCompaniesAsync(CompanyId[] sortedCompanyIds)
        {
            var companies = await repository.GetCompaniesAsync();
            var idsInDB = companies.Select(a => a.Id);
            if (!idsInDB.SequenceIgnoredEqual(sortedCompanyIds))
            {
                throw new Exception("提交的待排序Id中必须是所有的分类Id");
            }
            int seqNum = 1;
            //一个in语句一次性取出来更快，不过在非性能关键节点，业务语言比性能更重要
            foreach (CompanyId CompanyId in sortedCompanyIds)
            {
                var company = await repository.GetCompanyByIdAsync(CompanyId);
                if (company == null)
                {
                    throw new Exception($" CompanyId = {CompanyId} 不存在");
                }
                company.ChangeSequenceNumber(seqNum);//顺序改序号
                seqNum++;
            }
        }



        public async Task<Game> AddGameAsync(CompanyId companyId, MultilingualString title, string introduction, Uri coverUrl, DateTimeOffset releaseDate)
        {
            int maxSeq = await repository.GetMaxSequenceNumberOfGamesAsync(companyId);
            var id = new GameId(Guid.NewGuid());
            return Game.Create(companyId, id, title, introduction, coverUrl, releaseDate, maxSeq + 1);
        }

        public async Task SortGamesAsync(CompanyId companyId, GameId[] sortedGameIds)
        {
            var games = await repository.GetGamesByCompanyIdAsync(companyId);
            var idsInDB = games.Select(a => a.Id);
            if (!idsInDB.SequenceIgnoredEqual(sortedGameIds))
            {
                throw new Exception($"提交的待排序Id中必须是 companyId = {companyId} 分类下所有的Id");
            }

            int seqNum = 1;
            //一个in语句一次性取出来更快，不过在非性能关键节点，业务语言比性能更重要
            foreach (GameId gameId in sortedGameIds)
            {
                var game = await repository.GetGameByIdAsync(gameId);
                if (game is null)
                {
                    throw new Exception($"gameId = {gameId} 不存在");
                }
                game.ChangeSequenceNumber(seqNum);//顺序改序号
                seqNum++;
            }
        }



        public async Task<Exhibit> AddExhibitAsync(GameId gameId, Uri itemUrl)
        {
            int maxSeq = await repository.GetMaxSequenceNumberOfExhibitsAsync(gameId);
            var id = new ExhibitId(Guid.NewGuid());
            return new Exhibit(gameId, id, itemUrl, maxSeq + 1);
        }

        public async Task SortExhibitsAsync(GameId gameId, ExhibitId[] sortedExhibitIds)
        {
            var exhibits = await repository.GetExhibitsByGameIdAsync(gameId);
            var idsInDB = exhibits.Select(a => a.Id);
            if (!idsInDB.SequenceIgnoredEqual(sortedExhibitIds))
            {
                throw new Exception($"提交的待排序Id中必须是 gameId = {gameId} 分类下所有的Id");
            }

            int seqNum = 1;
            //一个in语句一次性取出来更快，不过在非性能关键节点，业务语言比性能更重要
            foreach (ExhibitId exhibitId in sortedExhibitIds)
            {
                var exhibit = await repository.GetExhibitByIdAsync(exhibitId);
                if (exhibit is null)
                {
                    throw new Exception($"tagId = {exhibitId} 不存在");
                }
                exhibit.ChangeSequenceNumber(seqNum);//顺序改序号
                seqNum++;
            }
        }

        public async Task<Tag> AddTagAsync(GameId gameId, string text)
        {
            int maxSeq = await repository.GetMaxSequenceNumberOfTagsAsync(gameId);
            var id = new TagId(Guid.NewGuid());
            return new Tag(id, text, maxSeq + 1);
        }

        public async Task SortTagsAsync(GameId gameId, TagId[] sortedTagIds)
        {
            var games = await repository.GetTagsByGameIdAsync(gameId);
            var idsInDB = games.Select(a => a.Id);
            if (!idsInDB.SequenceIgnoredEqual(sortedTagIds))
            {
                throw new Exception($"提交的待排序Id中必须是 gameId = {gameId} 分类下所有的Id");
            }

            int seqNum = 1;
            //一个in语句一次性取出来更快，不过在非性能关键节点，业务语言比性能更重要
            foreach (TagId tagId in sortedTagIds)
            {
                var tag = await repository.GetTagByIdAsync(tagId);
                if (tag is null)
                {
                    throw new Exception($"tagId = {tagId} 不存在");
                }
                tag.ChangeSequenceNumber(seqNum);//顺序改序号
                seqNum++;
            }
        }


       





    }
}
