using Showcase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showcase.Domain
{
    public interface IShowcaseRepository
    {
        Task<Company?> GetCompanyByIdAsync(CompanyId companyId);
        Task<Company[]> GetCompaniesAsync();
        Task<int> GetMaxSequenceNumberOfCompaniesAsync();


        public Task<Game?> GetGameByIdAsync(GameId gameId);
        public Task<int> GetMaxSequenceNumberOfGamesAsync(CompanyId companyId);
        public Task<Game[]> GetGamesByCompanyIdAsync(CompanyId companyId);

        public Task<Exhibit?> GetExhibitByIdAsync(ExhibitId exhibitId);
        public Task<int> GetMaxSequenceNumberOfExhibitsAsync(GameId gameId);
        public Task<Exhibit[]> GetExhibitsByGameIdAsync(GameId gameId);

        public Task<Tag[]> GetTagsByGameIdAsync(GameId gameId);



    }
}
