using AutoMapper;
using GameManagement.Api.Services;
using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;
using GameManagement.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace GameManagement.Api.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITagRepository tagRepository;

        public TagsController(IMapper mapper, ITagRepository tagRepository)
        {
            this.mapper = mapper;
            this.tagRepository = tagRepository;
        }


        [HttpGet(Name = nameof(GetGamesByTag))]
        [HttpHead]
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


        private IEnumerable<LinkDto> CreateLinksForTag(TagDtoParameters parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDto>();


            links.Add(new LinkDto(CreateTagsResourceUri(parameters, ResourceUriType.CurrentPage),
                "self", "GET"));

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
