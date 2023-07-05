using GameManagement.Shared.DataAccess;
using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;
using GameManagement.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Api.Services
{
    public class GameRepository : IGameRepository
    {
        private readonly GameManagementDbContext context;
        private readonly IPropertyMappingService propertyMappingService;

        public GameRepository(GameManagementDbContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context;
            this.propertyMappingService = propertyMappingService;
        }


        public async Task<IEnumerable<Game>> GetGamesAsync(Guid companyId, GameDtoParameters parameters)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var items = context.Games.Where(x => x.CompanyId == companyId);



            if (!string.IsNullOrWhiteSpace(parameters.Q))
            {
                parameters.Q = parameters.Q.Trim();

                items = items.Where(x => x.Title.Contains(parameters.Q)
                                         || x.Subtitle.Contains(parameters.Q)
                                         || x.Tags.Select(x => x.Content).Contains(parameters.Q));
            }

            var mappingDictionary = propertyMappingService.GetPropertyMapping<GameDto, Game>();

            items = items.ApplySort(parameters.OrderBy, mappingDictionary);

            return await items.ToListAsync();
        }

        public async Task<Game?> GetGameAsync(Guid companyId, Guid gameId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (gameId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(gameId));
            }

            return await context.Games
                .Where(x => x.CompanyId == companyId && x.Id == gameId)
                .FirstOrDefaultAsync();
        }


        public void AddGame(Guid companyId, Game game)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }
            if (game.Id == Guid.Empty)
            {
                game.Id = Guid.NewGuid();
            }

            game.CompanyId = companyId;
            game.CreateDate = DateTimeOffset.UtcNow;
            game.UpdateDate = DateTimeOffset.UtcNow;
            context.Games.Add(game);
        }


        public void UpdateGame(Game game)
        {
            game.UpdateDate = DateTimeOffset.UtcNow;
            // context.Entry(game).State = EntityState.Modified;
        }

        public void DeleteGame(Game game)
        {
            context.Games.Remove(game);
        }
        public async Task<bool> CompanyExistsAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await context.Companies.AnyAsync(x => x.Id == companyId);
        }



        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() >= 0;
        }


    }
}
