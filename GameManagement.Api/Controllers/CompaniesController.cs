﻿using AutoMapper;
using GameManagement.Api.ActionConstraints;
using GameManagement.Api.Services;
using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;
using GameManagement.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace GameManagement.Api.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IMapper mapper;
        private readonly ILogger<CompaniesController> logger;
        private readonly IPropertyMappingService propertyMappingService;
        private readonly IPropertyCheckerService propertyCheckerService;

        public CompaniesController(
            IMapper mapper,
            ILogger<CompaniesController> logger,
            ICompanyRepository companyRepository,
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            this.propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            this.propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        /// <summary>
        /// GetCompanies
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetCompanies))]
        [HttpHead]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyDtoParameters parameters)
        {
            if (!propertyMappingService.ValidMappingExistsFor<CompanyDto, Company>(parameters.OrderBy))
            {
                return BadRequest();
            }
            if (!propertyCheckerService.TypeHasProperties<CompanyDto>(parameters.Fields))
            {
                return BadRequest();
            }

            var companies = await companyRepository.GetCompaniesAsync(parameters);

            var paginationMetadata = new
            {
                totalCount = companies.TotalCount,
                pageSize = companies.PageSize,
                currentPage = companies.CurrentPage,
                totalPages = companies.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata,
                new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));

            var companyDtos = mapper.Map<IEnumerable<CompanyDto>>(companies);
            var shapedData = companyDtos.ShapeData(parameters.Fields);

            var links = CreateLinksForCompany(parameters, companies.HasPrevious, companies.HasNext);

            // { value: [xxx], links }

            var shapedCompaniesWithLinks = shapedData.Select(c =>
            {
                var companyDict = c as IDictionary<string, object>;
                var companyLinks = CreateLinksForCompany((Guid)companyDict["Id"], null);
                //var companyLinks = CreateLinksForTag((Guid)companyDict["Id"], companyDict["Fields"]);
                companyDict.Add("links", companyLinks);
                return companyDict;
            });

            var linkedCollectionResource = new
            {
                value = shapedCompaniesWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        /// <summary>
        /// GetCompany
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="fields"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        [Produces("application/json",
    "application/vnd.company.hateoas+json",
       "application/vnd.mycompany.company.friendly+json",
       "application/vnd.mycompany.company.friendly.hateoas+json",
       "application/vnd.mycompany.company.full+json",
       "application/vnd.mycompany.company.full.hateoas+json")]
        [HttpGet("{companyId}", Name = nameof(GetCompany))]
        public async Task<IActionResult> GetCompany(Guid companyId, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? parsedMediaType))
            {
                return BadRequest();
            }

            if (!propertyCheckerService.TypeHasProperties<CompanyDto>(fields))
            {
                return BadRequest();
            }
            var company = await companyRepository.GetCompanyAsync(companyId);

            if (company == null)
            {
                return NotFound();
            }

            var includeLinks =
                parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> myLinks = new List<LinkDto>();

            if (includeLinks)
            {
                myLinks = CreateLinksForCompany(companyId, fields);
            }

            var primaryMediaType = includeLinks
                ? parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.mycompany.company.full")
            {
                var full = mapper.Map<CompanyFullDto>(company)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    full.Add("links", myLinks);
                }

                return Ok(full);
            }

            var friendly = mapper.Map<CompanyDto>(company)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendly.Add("links", myLinks);
            }

            return Ok(friendly);
        }

        /// <summary>
        /// CreateCompanyWithBankruptTime
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost(Name = nameof(CreateCompanyWithBankruptTime))]
        [RequestHeaderMatchesMediaType("Content-Type", "application/vnd.mycompany.companyforcreationwithbankrupttime+json")]
        [Consumes("application/vnd.mycompany.companyforcreationwithbankrupttime+json")]
        public async Task<ActionResult<CompanyDto>> CreateCompanyWithBankruptTime(CompanyAddWithBankruptTimeDto company)
        {
            var entity = mapper.Map<Company>(company);
            companyRepository.AddCompany(entity);
            await companyRepository.SaveAsync();

            var returnDto = mapper.Map<CompanyDto>(entity);

            var links = CreateLinksForCompany(returnDto.Id, null);
            var linkedDict = returnDto.ShapeData(null)
                as IDictionary<string, object>;

            linkedDict.Add("links", links);

            return CreatedAtRoute(nameof(GetCompany), new { companyId = linkedDict["Id"] },
                linkedDict);
        }

        /// <summary>
        /// CreateCompany
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost(Name = nameof(CreateCompany))]
        [RequestHeaderMatchesMediaType("Content-Type", "application/json",
    "application/vnd.mycompany.companyforcreation+json")]
        [Consumes("application/json", "application/vnd.mycompany.companyforcreation+json")]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyAddDto company)
        {
            var entity = mapper.Map<Company>(company);
            companyRepository.AddCompany(entity);
            await companyRepository.SaveAsync();

            var returnDto = mapper.Map<CompanyDto>(entity);

            var links = CreateLinksForCompany(returnDto.Id, null);
            var linkedDict = returnDto.ShapeData(null)
                as IDictionary<string, object>;

            linkedDict.Add("links", links);

            return CreatedAtRoute(nameof(GetCompany), new { companyId = linkedDict["Id"] },
                linkedDict);
        }

        /// <summary>
        /// DeleteCompany
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpDelete("{companyId}", Name = nameof(DeleteCompany))]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var companyEntity = await companyRepository.GetCompanyAsync(companyId);

            if (companyEntity == null)
            {
                return NotFound();
            }

            companyRepository.DeleteCompany(companyEntity);
            await companyRepository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        /// GetCompaniesOptions
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,OPTIONS");
            return Ok();
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId, string? fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link(nameof(GetCompany), new { companyId }),
                        "self",
                        "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link(nameof(GetCompany), new { companyId, fields }),
                        "self",
                        "GET"));
            }

            links.Add(
                new LinkDto(Url.Link(nameof(DeleteCompany), new { companyId }),
                    "delete_company",
                    "DELETE"));

            links.Add(
                new LinkDto(Url.Link(nameof(GamesController.CreateGameForCompany), new { companyId }),
                    "create_game_for_company",
                    "POST"));

            links.Add(
                new LinkDto(Url.Link(nameof(GamesController.CreateGameForCompany), new { companyId }),
                    "games",
                    "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(CompanyDtoParameters parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.CurrentPage),
                "self", "GET"));

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage),
                    "previous_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage),
                    "next_page", "GET"));
            }

            return links;
        }

        private string? CreateCompaniesResourceUri(CompanyDtoParameters parameters, ResourceUriType type)
        {
            return type switch
            {
                ResourceUriType.PreviousPage => Url.Link(nameof(GetCompanies), new
                {
                    fields = parameters.Fields,
                    orderBy = parameters.OrderBy,
                    pageNumber = parameters.PageNumber - 1,
                    pageSize = parameters.PageSize,
                    companyName = parameters.CompanyName,
                    searchTerm = parameters.SearchTerm
                }),
                ResourceUriType.NextPage => Url.Link(nameof(GetCompanies), new
                {
                    fields = parameters.Fields,
                    orderBy = parameters.OrderBy,
                    pageNumber = parameters.PageNumber + 1,
                    pageSize = parameters.PageSize,
                    companyName = parameters.CompanyName,
                    searchTerm = parameters.SearchTerm
                }),
                _ => Url.Link(nameof(GetCompanies), new
                {
                    fields = parameters.Fields,
                    orderBy = parameters.OrderBy,
                    pageNumber = parameters.PageNumber,
                    pageSize = parameters.PageSize,
                    companyName = parameters.CompanyName,
                    searchTerm = parameters.SearchTerm
                }),
            };
        }
    }
}