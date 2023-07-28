using ASPNETCore;
using Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Showcase.Admin.WebAPI.Controllers.Companies.Requests;
using Showcase.Domain;
using Showcase.Domain.Entities;
using Showcase.Infrastructure;

namespace Showcase.Admin.WebAPI.Controllers.Companies
{
    [Route("api/companies")]
    [ApiController]
    [Authorize(Roles = UserRoles.Administrator)]
    [UnitOfWork(typeof(ShowcaseDbContext))]
    public class CompaniesController : ControllerBase
    {
        private readonly ShowcaseDbContext dbContext;
        private readonly ShowcaseDomainService domainService;
        private readonly IShowcaseRepository repository;

        public CompaniesController(ShowcaseDbContext dbContext, ShowcaseDomainService domainService, IShowcaseRepository repository)
        {
            this.dbContext = dbContext;
            this.domainService = domainService;
            this.repository = repository;
        }

        [HttpGet]
        public Task<Company[]> FindAll()
        {
            return repository.GetCompaniesAsync();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Company?>> FindById([RequiredStronglyType] CompanyId id)
        {
            var company = await repository.GetCompanyByIdAsync(id);
            if (company is null)
            {
                return NotFound($"没有 Id={id} 的 Company");
            }

            return company;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Add(CompanyAddRequest request)
        {
            var company = await domainService.AddCompanyAsync(request.Name, request.CoverUrl);
            dbContext.Add(company);//由于dbContext还没保存，不重定向了
            return company.Id.Value;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Update([RequiredStronglyType] CompanyId id, CompanyUpdateRequest request)
        {
            var company = await repository.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound("id 不存在");
            }
            company.ChangeName(request.Name);
            company.ChangeCoverUrl(request.CoverUrl);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteById([RequiredStronglyType] CompanyId id)
        {
            var company = await repository.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound($"没有 Id={id} 的 Company");
            }
            company.SoftDelete();//软删除
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Sort(CompaniesSortRequest request)
        {
            await domainService.SortCompaniesAsync(request.SortedCompanyIds);
            return Ok();
        }
    }
}