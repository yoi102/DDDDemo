using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;

namespace GameManagement.Api.Services
{
    public interface IGameRepository
    {
        Task<IEnumerable<Game>> GetGamesAsync(Guid companyId, GameDtoParameters parameters);
        Task<Game?> GetGameAsync(Guid companyId, Guid gameId);
        void AddGame(Guid companyId, Game game);
        void UpdateGame(Game game);
        void DeleteGame(Game game);
        Task<bool> CompanyExistsAsync(Guid companyId);

        Task<bool> SaveAsync();
    }
}
