using ASPNETCore;
using Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Showcase.Admin.WebAPI.Controllers.Tags.Requests;
using Showcase.Domain;
using Showcase.Domain.Entities;
using Showcase.Infrastructure;

namespace Showcase.Admin.WebAPI.Controllers.Tags
{
    [Route("api/tags")]
    [ApiController]
    [Authorize(Roles = UserRoles.Administrator)]
    [UnitOfWork(typeof(ShowcaseDbContext))]
    public class TagsController : ControllerBase
    {
        private readonly ShowcaseDbContext dbContext;
        private readonly ShowcaseDomainService domainService;
        private readonly IShowcaseRepository repository;

        public TagsController(ShowcaseDbContext dbContext, ShowcaseDomainService domainService, IShowcaseRepository repository)
        {
            this.dbContext = dbContext;
            this.domainService = domainService;
            this.repository = repository;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Tag>> FindById([RequiredStronglyType] TagId id)
        {
            var tag = await repository.GetTagByIdAsync(id);
            if (tag is null)
            {
                return NotFound("没有 id = {id} 的 Tag");
            }
            return tag;
        }

        [HttpGet]
        public async Task<ActionResult<Tag?>> FindByText(string text)
        {
            var tag = await repository.GetTagByTextAsync(text);
            return tag;
        }

        [HttpGet]
        [Route("gameId/{gameId}")]
        public Task<Tag[]> FindByCategoryId([RequiredStronglyType] GameId gameId)
        {
            return repository.GetTagsByGameIdAsync(gameId);
        }

        [HttpPost]
        public async Task<ActionResult<TagId>> Add(TagAddRequest request)
        {
            (Tag tag, bool has) = await domainService.AddTagAsync(request.GameId, request.Text);
            if (!has)
            {
                dbContext.Add(tag);
            }
            return tag.Id;
        }

        [HttpDelete]
        [Route("{gameId}/{id}")]
        public async Task<ActionResult> RemoveGameTagById([RequiredStronglyType] GameId gameId, [RequiredStronglyType] TagId id)
        {
            var game = await repository.GetGameByIdAsync(gameId);
            if (game is null)
            {
                return NotFound($"没有 gameId={gameId} 的 Game");
            }
            var tag = await repository.GetTagByIdAsync(id);

            if (tag is null)
            {
                return NotFound($"没有 Id={id} 的 Tag");
            }
            var gameTag = await repository.GetGameTagByIdAsync(gameId, id);
            if (gameTag is not null)
            {
                dbContext.Remove(gameTag);
            }

            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Update([RequiredStronglyType] TagId id, string text)
        {
            var tag = await repository.GetTagByIdAsync(id);
            if (tag is null)
            {
                return NotFound($"没有 Id={id} 的 Tag");
            }
            tag.ChangeText(text);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteById([RequiredStronglyType] TagId id)
        {
            var tag = await repository.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound($"没有 Id={id} 的 Tag");
            }
            dbContext.Remove(tag);
            return Ok();
        }
    }
}