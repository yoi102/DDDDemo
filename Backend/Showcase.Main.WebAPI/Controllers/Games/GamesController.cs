using ASPNETCore;
using Microsoft.AspNetCore.Mvc;
using Showcase.Domain;
using Showcase.Domain.Entities;
using Showcase.Main.WebAPI.Controllers.Games.ViewModels;

namespace Showcase.Main.WebAPI.Controllers.Games
{
    [Route("api/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IShowcaseRepository repository;
        private readonly IMemoryCacheHelper memoryCache;

        public GamesController(IShowcaseRepository repository, IMemoryCacheHelper memoryCache)
        {
            this.repository = repository;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("{companyId}")]
        public async Task<ActionResult<GameViewModel[]?>> FindByCompanyId([RequiredStronglyType] CompanyId companyId)
        {
            //写到单独的 local 函数的好处是避免回调中代码太复杂
            Task<Game[]> FindDataAsync()
            {
                return repository.GetGamesByCompanyIdAsync(companyId);
            }
            var task = memoryCache.GetOrCreateAsync($"GamesController.FindByCompanyId.{companyId}",
                async (e) => GameViewModel.Create(await FindDataAsync()));
            return await task;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<GameViewModel>> FindById([RequiredStronglyType] GameId id)
        {
            var viewModel = await memoryCache.GetOrCreateAsync($"GamesController.FindById.{id}",
               async (e) => GameViewModel.Create(await repository.GetGameByIdAsync(id)));
            if (viewModel == null)
            {
                return NotFound($"没有 Id={id} 的 Game");
            }
            return viewModel;
        }
    }
}