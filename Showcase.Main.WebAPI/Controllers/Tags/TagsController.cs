using ASPNETCore;
using Microsoft.AspNetCore.Mvc;
using Showcase.Domain;
using Showcase.Domain.Entities;
using Showcase.Main.WebAPI.Controllers.Tags.ViewModels;

namespace Showcase.Main.WebAPI.Controllers.Tags
{
    [Route("api/tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IShowcaseRepository repository;
        private readonly IMemoryCacheHelper memoryCache;

        public TagsController(IShowcaseRepository repository, IMemoryCacheHelper memoryCache)
        {
            this.repository = repository;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("{gameId}")]
        public async Task<ActionResult<TagViewModel[]?>> FindByGameId([RequiredStronglyType] GameId gameId)
        {
            Task<Tag[]> FindDataAsync()
            {
                return repository.GetTagsByGameIdAsync(gameId);
            }

            var task = memoryCache.GetOrCreateAsync($"TagsController.FindByGameId.{gameId}",
                async (e) => TagViewModel.Create(await FindDataAsync()));
            return await task;
        }

        [HttpGet]
        [Route("{Text}")]
        public async Task<ActionResult<TagViewModel>> FindById([RequiredStronglyType] TagId id)
        {
            var viewModel = await memoryCache.GetOrCreateAsync($"TagsController.FindById.{id}",
                async (e) => TagViewModel.Create(await repository.GetTagByIdAsync(id)));
            if (viewModel == null)
            {
                return NotFound($"没有 Id={id} 的 Tag");
            }
            return viewModel;
        }

        [HttpGet]
        public async Task<ActionResult<TagViewModel>> FindByText([RequiredStronglyType] string Text)
        {
            var viewModel = await memoryCache.GetOrCreateAsync($"TagsController.FindByText.{Text}",
                async (e) => TagViewModel.Create(await repository.GetTagByTextAsync(Text)));
            if (viewModel == null)
            {
                return NotFound($"没有 {Text} 的 Tag");
            }
            return viewModel;
        }
    }
}