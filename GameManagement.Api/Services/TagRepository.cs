using GameManagement.Shared.DataAccess;
using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;

namespace GameManagement.Api.Services
{
    public class TagRepository : ITagRepository
    {
        private readonly GameManagementDbContext context;

        public TagRepository(GameManagementDbContext context)
        {
            this.context = context;
        }

        public async Task<PagedList<Game>?> GetGamesAsync(TagDtoParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.Tag))
            {
                return null;
            }

            var queryExpression = context.Tags.Where(x => x.Content.Contains(parameters.Tag)).SelectMany(x => x.Games);

            return queryExpression is null
                ? null
                : await PagedList<Game>.CreateAsync(queryExpression, parameters.PageNumber, parameters.PageSize);
        }
    }
}