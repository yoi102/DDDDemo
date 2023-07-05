using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;

namespace GameManagement.Api.Services
{
    public interface ITagRepository
    {
        Task<PagedList<Game>?> GetGamesAsync(TagDtoParameters parameters);

    }
}
