using GameManagement.Shared.DataAccess;
using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;
using GameManagement.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Api.Services
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly GameManagementDbContext context;
        private readonly IPropertyMappingService propertyMappingService;

        public CompanyRepository(GameManagementDbContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public async Task<PagedList<Company>> GetCompaniesAsync(CompanyDtoParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var queryExpression = context.Companies as IQueryable<Company>;

            if (!string.IsNullOrWhiteSpace(parameters.CompanyName))
            {
                parameters.CompanyName = parameters.CompanyName.Trim();
                queryExpression = queryExpression.Where(x => x.Name == parameters.CompanyName);
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                parameters.SearchTerm = parameters.SearchTerm.Trim();
                queryExpression = queryExpression.Where(x => x.Name.Contains(parameters.SearchTerm) ||
                                                             x.Introduction.Contains(parameters.SearchTerm));
            }

            var mappingDictionary = propertyMappingService.GetPropertyMapping<CompanyDto, Company>();

            queryExpression = queryExpression.ApplySort(parameters.OrderBy, mappingDictionary);

            return await PagedList<Company>.CreateAsync(queryExpression, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<Company> GetCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await context.Companies
                .FirstOrDefaultAsync(x => x.Id == companyId);
        }

        public async Task<IEnumerable<Company>>
            GetCompaniesAsync(IEnumerable<Guid> companyIds)
        {
            if (companyIds == null)
            {
                throw new ArgumentNullException(nameof(companyIds));
            }

            return await context.Companies
                .Where(x => companyIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public void AddCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            company.Id = Guid.NewGuid();

            if (company.Games != null)
            {
                foreach (var employee in company.Games)
                {
                    employee.Id = Guid.NewGuid();
                }
            }

            context.Companies.Add(company);
        }

        public void UpdateCompany(Company company)
        {
            // context.Entry(company).State = EntityState.Modified;
        }

        public void DeleteCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            context.Companies.Remove(company);
        }

        public async Task<bool> CompanyExistsAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await context.Companies.AnyAsync(x => x.Id == companyId);
        }

        public async Task<IEnumerable<Game>> GetEmployeesAsync(Guid companyId,
            GameDtoParameters parameters)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var items = context.Games.Where(x => x.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(parameters.Gender))
            {
                parameters.Gender = parameters.Gender.Trim();
                var gender = Enum.Parse<Gender>(parameters.Gender);

                items = items.Where(x => x.Gender == gender);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Q))
            {
                parameters.Q = parameters.Q.Trim();

                items = items.Where(x => x.EmployeeNo.Contains(parameters.Q)
                                         || x.FirstName.Contains(parameters.Q)
                                         || x.LastName.Contains(parameters.Q));
            }

            var mappingDictionary = propertyMappingService.GetPropertyMapping<GameDto, Employee>();

            items = items.ApplySort(parameters.OrderBy, mappingDictionary);

            return await items.ToListAsync();
        }

        public async Task<Game> GetEmployeeAsync(Guid companyId, Guid employeeId)
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
