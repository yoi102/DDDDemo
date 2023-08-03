using ASPNETCore;
using Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Showcase.Admin.WebAPI.Controllers.Exhibits.Request;
using Showcase.Domain;
using Showcase.Domain.Entities;
using Showcase.Infrastructure;

namespace Showcase.Admin.WebAPI.Controllers.Exhibits
{
    [Route("api/exhibits")]
    [ApiController]
    [Authorize(Roles = UserRoles.Administrator)]
    [UnitOfWork(typeof(ShowcaseDbContext))]
    public class ExhibitsController : ControllerBase
    {
        private readonly ShowcaseDbContext dbContext;
        private readonly ShowcaseDomainService domainService;
        private readonly IShowcaseRepository repository;

        public ExhibitsController(ShowcaseDbContext dbContext, ShowcaseDomainService domainService, IShowcaseRepository repository)
        {
            this.dbContext = dbContext;
            this.domainService = domainService;
            this.repository = repository;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Exhibit>> FindById([RequiredStronglyType] ExhibitId id)
        {
            var exhibit = await repository.GetExhibitByIdAsync(id);
            if (exhibit is null)
            {
                return NotFound("没有 id={id} 的 Exhibit");
            }
            return exhibit;
        }

        [HttpGet]
        [Route("gameId/{gameId}")]
        public Task<Exhibit[]> FindByCategoryId([RequiredStronglyType] GameId gameId)
        {
            return repository.GetExhibitsByGameIdAsync(gameId);
        }

        [HttpPost]
        public async Task<ActionResult<ExhibitId>> Add(ExhibitAddRequest request)
        {
            var exhibit = await domainService.AddExhibitAsync(request.GameId, request.ItemUrl);
            dbContext.Add(exhibit);
            return exhibit.Id;
        }

        [HttpPut]
        [Route("id/{id}")]
        public async Task<ActionResult> Update([RequiredStronglyType] ExhibitId id, ExhibitUpdateRequest request)
        {
            var exhibit = await repository.GetExhibitByIdAsync(id);
            if (exhibit is null)
            {
                return NotFound("没有 id=={id} 的 Exhibit");
            }
            exhibit.ChangeItemUrl(request.ItemUrl);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteById([RequiredStronglyType] ExhibitId id)
        {
            var exhibit = await repository.GetExhibitByIdAsync(id);
            if (exhibit == null)
            {
                return NotFound($"没有 Id={id} 的 Exhibit");
            }
            exhibit.SoftDelete();//软删除
            return Ok();
        }

        [HttpPut]
        [Route("gameId/{gameId}")]
        public async Task<ActionResult> Sort([RequiredStronglyType] GameId gameId, ExhibitsSortRequest request)
        {
            await domainService.SortExhibitsAsync(gameId, request.SortedExhibitIds);
            return Ok();
        }
    }
}