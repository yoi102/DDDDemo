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


        public async Task<IEnumerable<Game>> GetEmployeesAsync(Guid companyId, GameDtoParameters parameters)
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
                                         || x.Tags.Select(x=>x.Content).Contains(parameters.Q));
            }

            var mappingDictionary = propertyMappingService.GetPropertyMapping<GameDto, Game>();

            items = items.ApplySort(parameters.OrderBy, mappingDictionary);

            return await items.ToListAsync();
        }

        public async Task<Game?> GetEmployeeAsync(Guid companyId, Guid employeeId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (employeeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(employeeId));
            }

            return await context.Games
                .Where(x => x.CompanyId == companyId && x.Id == employeeId)
                .FirstOrDefaultAsync();
        }

        public void AddEmployee(Guid companyId, Game game)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            game.CompanyId = companyId;
            context.Games.Add(game);
        }

        public void UpdateEmployee(Game game)
        {
            // context.Entry(game).State = EntityState.Modified;
        }

        public void DeleteEmployee(Game game)
        {
            context.Games.Remove(game);
        }

        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() >= 0;
        }


    }
}
