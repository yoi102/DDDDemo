﻿using ASPNETCore;
using Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Showcase.Admin.WebAPI.Controllers.Games.Requests;
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
        public async Task<ActionResult<Tag?>> FindById([RequiredStronglyType] TagId id)
        {
            var tag = await repository.GetTagByIdAsync(id);
            return tag;
        }

        [HttpGet]
        public async Task<ActionResult<Tag?>> FindByText(string text)
        {
            var tag = await repository.GetTagByTextAsync(text);
            return tag;
        }
        [HttpGet]
        [Route("{gameId}")]
        public Task<Tag[]> FindByCategoryId([RequiredStronglyType] GameId gameId)
        {
            return repository.GetTagsByGameIdAsync(gameId);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Add([RequiredStronglyType] GameId gameId, string text)
        {
            (Tag tag, bool has) = await domainService.AddTagAsync(gameId, text);
            if (!has)
            {
                dbContext.Add(tag);
            }
            return tag.Id.Value;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> RemoveGameTagById([RequiredStronglyType] GameId gameId, [RequiredStronglyType] TagId id)
        {
            var game = await repository.GetGameByIdAsync(gameId);
            if (game == null)
            {
                return NotFound($"没有Id={gameId} 的 Game");
            }
            if (!game.TagIds.Contains(id))
            {
                return NotFound($"Id={id} 的 Game，没有 Id={id} 的 Tag");

            }
            game.TagIds.Remove(id);
            return Ok();
        }



        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Update([RequiredStronglyType] TagId id, string text)
        {
            var tag = await repository.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound("id没找到");
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
                return NotFound($"没有Id={id} 的 Tag");
            }
            dbContext.Remove(tag);
            return Ok();
        }


    }
}