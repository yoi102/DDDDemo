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

        public void AddTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            tag.Id = Guid.NewGuid();
            tag.CreationTime = DateTimeOffset.Now;
            tag.UpdatedDate = DateTimeOffset.Now;
            if (tag.Games != null)
            {
                foreach (var game in tag.Games)
                {
                    game.Id = Guid.NewGuid();
                    game.CreationTime = DateTimeOffset.Now;
                    game.UpdatedDate = DateTimeOffset.Now;
                }
            }

            context.Tags.Add(tag);
        }

        public void DeleteTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            context.Tags.Remove(tag);
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

        public Task<Tag> GetTagAsync(Guid tagId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() >= 0;
        }
    }
}