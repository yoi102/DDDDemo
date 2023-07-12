using AutoMapper;
using GameManagement.Api.Services;
using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;
using GameManagement.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace GameManagement.Api.Controllers
{
    /// <summary>
    /// TagsController
    /// </summary>
    [Route("api/tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITagRepository tagRepository;
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="tagRepository"></param>
        /// <param name="memoryCache"></param>
        public TagsController(IMapper mapper, ITagRepository tagRepository,IMemoryCache memoryCache)
        {
            this.mapper = mapper;
            this.tagRepository = tagRepository;
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// GetGamesByTag
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetGamesByTag))]
        public async Task<IActionResult> GetGamesByTag([FromQuery] TagDtoParameters parameters)
        {
            var games = await tagRepository.GetGamesAsync(parameters);
            if (games is null)
            {
                return NotFound();
            }
            var paginationMetadata = new
            {
                totalCount = games.TotalCount,
                pageSize = games.PageSize,
                currentPage = games.CurrentPage,
                totalPages = games.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata,
                new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));

            var gameDtos = mapper.Map<IEnumerable<GameDto>>(games);

            var links = CreateLinksForTag(parameters, games.HasPrevious, games.HasNext);

            var linkedCollectionResource = new
            {
                value = gameDtos,
                links
            };

            return Ok(linkedCollectionResource);
        }
        /// <summary>
        /// GetTag
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        [HttpGet("{tagId}", Name = nameof(GetTag))]
        public async Task<IActionResult> GetTag(Guid tagId)
        {
            var tag = await tagRepository.GetTagAsync(tagId);

            if (tag == null)
            {
                return NotFound();
            }
            var dto = mapper.Map<TagDto>(tag);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag(AddTagDto tag)
        {
            var entity = mapper.Map<Tag>(tag);
            tagRepository.AddTag(entity);
            await tagRepository.SaveAsync();

            var returnDto = mapper.Map<CompanyDto>(entity);

            var links = CreateLinksForTag(returnDto.Id);
            var linkedDict = returnDto.ShapeData(null)
                as IDictionary<string, object>;

            linkedDict.Add("links", links);

            return CreatedAtRoute(nameof(GetTag), new { tagId = linkedDict["Id"] },
                linkedDict);
        }
        /// <summary>
        /// DeleteTag
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        [HttpDelete("{tagId}", Name = nameof(DeleteTag))]
        public async Task<IActionResult> DeleteTag(Guid tagId)
        {
            var entity = await tagRepository.GetTagAsync(tagId);

            if (entity == null)
            {
                return NotFound();
            }

            tagRepository.DeleteTag(entity);
            await tagRepository.SaveAsync();

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForTag(Guid tagId)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link(nameof(GetTag), new { tagId }),
                    "self",
                    "GET"));

            links.Add(
                new LinkDto(Url.Link(nameof(DeleteTag), new { tagId }),
                    "delete_tag",
                    "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTag(TagDtoParameters parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateTagsResourceUri(parameters, ResourceUriType.CurrentPage),
                "self", "GET")
            };

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateTagsResourceUri(parameters, ResourceUriType.PreviousPage),
                    "previous_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(new LinkDto(CreateTagsResourceUri(parameters, ResourceUriType.NextPage),
                    "next_page", "GET"));
            }

            return links;
        }

        private string? CreateTagsResourceUri(TagDtoParameters parameters, ResourceUriType type)
        {
            return type switch
            {
                ResourceUriType.PreviousPage => Url.Link(nameof(GetGamesByTag), new
                {
                    tag = parameters.Tag,
                    pageNumber = parameters.PageNumber - 1,
                    pageSize = parameters.PageSize,
                }),
                ResourceUriType.NextPage => Url.Link(nameof(GetGamesByTag), new
                {
                    tag = parameters.Tag,
                    pageNumber = parameters.PageNumber + 1,
                    pageSize = parameters.PageSize,
                }),
                _ => Url.Link(nameof(GetGamesByTag), new
                {
                    tag = parameters.Tag,
                    pageNumber = parameters.PageNumber,
                    pageSize = parameters.PageSize,
                }),
            };
        }
    }
}