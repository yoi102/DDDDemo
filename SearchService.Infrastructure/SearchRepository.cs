using Nest;
using SearchService.Domain;

namespace SearchService.Infrastructure
{
    public class SearchRepository : ISearchRepository
    {
        private readonly IElasticClient elasticClient;

        public SearchRepository(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }

        public Task DeleteAsync(Guid gameId)
        {
            elasticClient.DeleteByQuery<Game>(q => q
               .Index("games")
               .Query(rq => rq.Term(f => f.Id, "elasticsearch.pm")));
            //因为有可能文档不存在，所以不检查结果
            //如果 Game 被删除，则把对应的数据也从Elastic Search中删除
            return elasticClient.DeleteAsync(new DeleteRequest("games", gameId));
        }


        public async Task UpsertAsync(Game episode)
        {
            var response = await elasticClient.IndexAsync(episode, idx => idx.Index("games").Id(episode.Id));
            if (!response.IsValid)
            {
                throw new ApplicationException(response.DebugInformation);
            }
        }





        public async Task<SearchGamesResponse> SearchEpisodes(string keyword, int pageIndex, int pageSize)
        {
            int from = pageSize * (pageIndex - 1);
            string kw = keyword;
            Func<QueryContainerDescriptor<Game>, QueryContainer> query = (q) =>
                          q.Match(mq => mq.Field(f => f.Title.Chinese).Query(kw))
                          || q.Match(mq => mq.Field(f => f.Title.English).Query(kw))
                          || q.Match(mq => mq.Field(f => f.Title.Japanese).Query(kw))
                          || q.Match(mq => mq.Field(f => f.Introduction).Query(kw));
            Func<HighlightDescriptor<Game>, IHighlight> highlightSelector = h => h
                .Fields(fs => fs.Field(f => f.Introduction));
            var result = await this.elasticClient.SearchAsync<Game>(s => s.Index("games").From(from)
                .Size(pageSize).Query(query).Highlight(highlightSelector));
            if (!result.IsValid)
            {
                throw result.OriginalException;
            }
            List<Game> games = new List<Game>();
            foreach (var hit in result.Hits)
            {
                string highlightedSubtitle;
                //如果没有预览内容，则显示前50个字
                if (hit.Highlight.ContainsKey("introduction"))
                {
                    highlightedSubtitle = string.Join("\r\n", hit.Highlight["introduction"]);
                }
                else
                {
                    highlightedSubtitle = hit.Source.Introduction.Cut(50);
                }
                var game = hit.Source with { Introduction = highlightedSubtitle };
                games.Add(game);
            }
            return new SearchGamesResponse(games, result.Total);
        }












    }
}
