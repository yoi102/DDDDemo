using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;

namespace GameManagement.Api.Services
{
    public interface ITagRepository
    {
        void AddTag(Tag tag);
        void DeleteTag(Tag tag);
        Task<PagedList<Game>?> GetGamesAsync(TagDtoParameters parameters);
        Task<Tag> GetTagAsync(Guid tagId);
        Task<bool> SaveAsync();
    }
}