using ASPNETCore;
using Microsoft.AspNetCore.Mvc;
using Showcase.Domain;
using Showcase.Domain.Entities;
using Showcase.Main.WebAPI.Controllers.Exhibits.ViewModels;

namespace Showcase.Main.WebAPI.Controllers.Exhibits
{
    [Route("api/exhibits")]
    [ApiController]
    public class ExhibitsController : ControllerBase
    {
        private readonly IShowcaseRepository repository;
        private readonly IMemoryCacheHelper memoryCache;

        public ExhibitsController(IShowcaseRepository repository, IMemoryCacheHelper memoryCache)
        {
            this.repository = repository;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("{gameId}")]
        public async Task<ActionResult<ExhibitViewModel[]?>> FindByGameId([RequiredStronglyType] GameId gameId)
        {
            Task<Exhibit[]> FindDataAsync()
            {
                return repository.GetExhibitsByGameIdAsync(gameId);
            }
            var task = memoryCache.GetOrCreateAsync($"ExhibitsController.FindByGameId.{gameId}",
                async (e) => ExhibitViewModel.Create(await FindDataAsync()));
            return await task;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ExhibitViewModel>> FindById([RequiredStronglyType] ExhibitId id)
        {
            var viewModel = await memoryCache.GetOrCreateAsync($"ExhibitsController.FindById.{id}",
               async (e) => ExhibitViewModel.Create(await repository.GetExhibitByIdAsync(id)));
            if (viewModel == null)
            {
                return NotFound($"没有 Id={id} 的 Exhibit");
            }
            return viewModel;
        }
    }
}