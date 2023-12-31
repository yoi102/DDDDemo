﻿using ASPNETCore;
using Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Showcase.Admin.WebAPI.Controllers.Games.Requests;
using Showcase.Domain;
using Showcase.Domain.Entities;
using Showcase.Infrastructure;

namespace Showcase.Admin.WebAPI.Controllers.Games
{
    [Route("api/games")]
    [ApiController]
    [Authorize(Roles = UserRoles.Administrator)]
    [UnitOfWork(typeof(ShowcaseDbContext))]
    public class GamesController : ControllerBase
    {
        private readonly ShowcaseDbContext dbContext;
        private readonly ShowcaseDomainService domainService;
        private readonly IShowcaseRepository repository;

        public GamesController(ShowcaseDbContext dbContext, ShowcaseDomainService domainService, IShowcaseRepository repository)
        {
            this.dbContext = dbContext;
            this.domainService = domainService;
            this.repository = repository;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Game>> FindById([RequiredStronglyType] GameId id)
        {
            var game = await repository.GetGameByIdAsync(id);
            if (game is null)
            {
                return NotFound("没有 id = {id} 的 Game");
            }
            return game;
        }

        [HttpGet]
        [Route("companyId/{companyId}")]
        public Task<Game[]> FindByCategoryId([RequiredStronglyType] CompanyId companyId)
        {
            return repository.GetGamesByCompanyIdAsync(companyId);
        }

        [HttpPost]
        public async Task<ActionResult<GameId>> Add(GameAddRequest request)
        {
            var game = await domainService.AddGameAsync(request.CompanyId, request.Title, request.Introduction, request.CoverUrl, request.ReleaseDate);
            dbContext.Add(game);
            return game.Id;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Update([RequiredStronglyType] GameId id, GameUpdateRequest request)
        {
            var game = await repository.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound("没有 id = {id} 的 Game");
            }
            game.ChangeTitle(request.Title);
            game.ChangeCoverUrl(request.CoverUrl);
            game.ChangeIntroduction(request.Introduction);
            game.ChangeReleaseDate(request.ReleaseDate);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteById([RequiredStronglyType] GameId id)
        {
            var game = await repository.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound($"没有 Id={id} 的 Game");
            }
            game.SoftDelete();//软删除
            return Ok();
        }

        [HttpPut]
        [Route("companyId/{companyId}")]
        public async Task<ActionResult> Sort([RequiredStronglyType] CompanyId companyId, GamesSortRequest request)
        {
            await domainService.SortGamesAsync(companyId, request.SortedGameIds);
            return Ok();
        }
    }
}