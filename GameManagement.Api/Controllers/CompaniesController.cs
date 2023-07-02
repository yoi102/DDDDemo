using AutoMapper;
using GameManagement.Api.Services;
using GameManagement.Shared.DtoParameters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using GameManagement.Shared.Models;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;
using Microsoft.Net.Http.Headers;

namespace GameManagement.Api.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IMapper mapper;
        private readonly IPropertyMappingService propertyMappingService;
        private readonly IPropertyCheckerService propertyCheckerService;

        public CompaniesController(ICompanyRepository publisherRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            this.companyRepository = publisherRepository;
            this.mapper = mapper;
            this.propertyMappingService = propertyMappingService;
            this.propertyCheckerService = propertyCheckerService;
        }


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
                //var companyLinks = CreateLinksForCompany((Guid)companyDict["Id"], companyDict["Fields"]);
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




        [Produces("application/json",
    "application/vnd.company.hateoas+json",
    "application/vnd.mycompany.company.friendly+json",
    "application/vnd.mycompany.company.friendly.hateoas+json",
    "application/vnd.mycompany.company.full+json",
    "application/vnd.mycompany.company.full.hateoas+json")]
        [HttpGet("{companyId}", Name = nameof(GetCompany))]
        // [Route("{companyId}")]
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
            var publisher = await companyRepository.GetCompanyAsync(companyId);

            if (publisher == null)
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

            if (primaryMediaType == "vnd.mycompany.publisher.full")
            {
                var full = mapper.Map<CompanyFullDto>(publisher)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    full.Add("links", myLinks);
                }

                return Ok(full);
            }

            var friendly = mapper.Map<CompanyDto>(publisher)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendly.Add("links", myLinks);
            }

            return Ok(friendly);
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
                new LinkDto(Url.Link(nameof(DeletePublisher), new { companyId }),
                    "delete_publisher",
                    "DELETE"));

            links.Add(
                new LinkDto(Url.Link(nameof(GamesController.CreateGameForPublisher), new { companyId }),
                    "create_game_for_publisher",
                    "POST"));

            links.Add(
                new LinkDto(Url.Link(nameof(GamesController.CreateGameForPublisher), new { companyId }),
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
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });

                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });

                case ResourceUriType.CurrentPage:
                default:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
            }



        }









    }
}
