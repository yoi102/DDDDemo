using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SearchService.Domain;

namespace SearchService.Infrastructure
{
    public class SearchRepository : ISearchRepository
    {
        private readonly ElasticsearchClient elasticClient;

        private readonly string[] multiMatchFields = new string[] { "titleZh", "titleEn", "titleJa", "introduction" };

        public SearchRepository(ElasticsearchClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }

        public Task DeleteAsync(Guid gameId)
        {
            DeleteByQueryRequestDescriptor<Game> descriptor = new DeleteByQueryRequestDescriptor<Game>("games");

            elasticClient.DeleteByQuery<Game>(
                new DeleteByQueryRequestDescriptor<Game>("games")
                .Query(rq => rq.Term(f => f.Id, "elasticsearch.pm")));
            //因为有可能文档不存在，所以不检查结果
            //如果 Game 被删除，则把对应的数据也从Elastic Search中删除
            return elasticClient.DeleteAsync(new DeleteRequest("games", gameId));
        }

        public async Task UpsertAsync(Game game)
        {
            var response = await elasticClient.IndexAsync(game, idx => idx.Index("games").Id(game.Id));
            if (!response.IsValidResponse)
            {
                throw new ApplicationException(response.DebugInformation);
            }
        }

        public async Task<SearchGamesResponse?> SearchGames(string keyword, int pageIndex, int pageSize)
        {
            int from = pageSize * (pageIndex - 1);
            string kw = keyword;
            QueryDescriptor<Game> query = new QueryDescriptor<Game>();
            query.MultiMatch(x => x.Query(kw).Fuzziness(new Fuzziness("Auto"))
                                                                                              .Operator(Operator.And).Fields(multiMatchFields));

            //query.Match(x => x.Field(f => f.Title.Japanese).Query(kw));//Match 分词查找///...最后一个为准......
            //query.Term(f => f.Title.Japanese, kw);//Term 完全匹配.....最后一个为准......

            HighlightDescriptor<Game> highlightSelector = new HighlightDescriptor<Game>();
            highlightSelector.HighlightQuery(query).Fields(x =>
            {
                return x;
            });//..............

            var result = await elasticClient.SearchAsync<Game>(s => s.Index("games").From(from)
                .Size(pageSize).Query(query).Highlight(highlightSelector));

            if (result.IsValidResponse)
            {
                List<Game> games = new List<Game>();
                foreach (var hit in result.Hits)
                {
                    string highlightedIntroduction;
                    //如果没有预览内容，则显示前50个字
                    if (hit.Highlight is not null && hit.Highlight.ContainsKey("introduction"))
                    {
                        highlightedIntroduction = string.Join("\r\n", hit.Highlight["introduction"]);
                    }
                    else
                    {
                        highlightedIntroduction = hit.Source!.Introduction.Cut(50);
                    }
                    var game = hit.Source! with { Introduction = highlightedIntroduction };
                    games.Add(game!);
                }
                return new SearchGamesResponse(games, result.Total);
            }
            return null;
        }
    }
}