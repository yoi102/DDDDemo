using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;

namespace GameManagement.Api.Services
{
    public interface IGameRepository
    {
        Task<IEnumerable<Game>> GetEmployeesAsync(Guid companyId, GameDtoParameters parameters);
        Task<Game> GetEmployeeAsync(Guid companyId, Guid employeeId);
        void AddEmployee(Guid companyId, Game employee);
        void UpdateEmployee(Game employee);
        void DeleteEmployee(Game employee);

        Task<bool> SaveAsync();
    }
}
