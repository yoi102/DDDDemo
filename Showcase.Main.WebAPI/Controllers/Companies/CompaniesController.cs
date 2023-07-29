using ASPNETCore;
using Microsoft.AspNetCore.Mvc;
using Showcase.Domain;
using Showcase.Domain.Entities;
using Showcase.Main.WebAPI.Controllers.Companies.ViewModels;

namespace Showcase.Main.WebAPI.Controllers.Companies
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IShowcaseRepository repository;
        private readonly IMemoryCacheHelper memoryCache;

        public CompaniesController(IShowcaseRepository repository, IMemoryCacheHelper memoryCache)
        {
            this.repository = repository;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<CompanyViewModel[]?>> FindAll()
        {
            Task<Company[]> FindDataAsync()
            {
                return repository.GetCompaniesAsync();
            }
            var task = memoryCache.GetOrCreateAsync($"CompaniesController.FindAll",
                async (e) => CompanyViewModel.Create(await FindDataAsync()));
            return await task;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CompanyViewModel?>> FindById([RequiredStronglyType] CompanyId id)
        {
            var viewModel = await memoryCache.GetOrCreateAsync($"CompaniesController.FindById.{id}",
                async (e) => CompanyViewModel.Create(await repository.GetCompanyByIdAsync(id)));
            if (viewModel == null)
            {
                return NotFound($"没有 Id={id} 的 Company");
            }
            else
            {
                return viewModel;
            }
        }
    }
}