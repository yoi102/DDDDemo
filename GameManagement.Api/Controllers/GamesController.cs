using AutoMapper;
using GameManagement.Api.Services;
using GameManagement.Shared.DtoParameters;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace GameManagement.Api.Controllers
{
    [Route("api/companies/{companiesId}/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IGameRepository gameRepository;

        public GamesController(IMapper mapper, IGameRepository gameRepository)
        {
            this.mapper = mapper;
            this.gameRepository = gameRepository;
        }

        [HttpGet(Name = nameof(GetGamesForCompany))]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesForCompany(Guid companyId, [FromQuery] GameDtoParameters parameters)
        {
            if (!await gameRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var games = await gameRepository
                .GetGamesAsync(companyId, parameters);

            var gameDtos = mapper.Map<IEnumerable<GameDto>>(games);

            return Ok(gameDtos);
        }

        [HttpGet("{gameId}", Name = nameof(GetGameForCompany))]
        //[ResponseCache(Duration = 60)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1800)]
        [HttpCacheValidation(MustRevalidate = true)]
        public async Task<ActionResult<GameDto>> GetGameForCompany(Guid companyId, Guid gameId)
        {
            if (!await gameRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var game = await gameRepository.GetGameAsync(companyId, gameId);
            if (game == null)
            {
                return NotFound();
            }

            var gameDto = mapper.Map<GameDto>(game);

            return Ok(gameDto);
        }

        [HttpPost(Name = nameof(CreateGameForCompany))]
        public async Task<ActionResult<GameDto>> CreateGameForCompany(Guid companyId, GameAddDto game)
        {
            if (!await gameRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var entity = mapper.Map<Game>(game);

            gameRepository.AddGame(companyId, entity);
            await gameRepository.SaveAsync();

            var dtoToReturn = mapper.Map<GameDto>(entity);

            return CreatedAtRoute(nameof(GetGameForCompany), new
            {
                companyId,
                gameId = dtoToReturn.Id
            }, dtoToReturn);
        }

        [HttpPut("{gameId}")]//todo 文件上传。。。  IFormFile
        public async Task<ActionResult<GameDto>> UpdateGameForCompany(Guid companyId, Guid gameId, GameUpdateDto game)
        {
            if (!await gameRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var gameEntity = await gameRepository.GetGameAsync(companyId, gameId);

            if (gameEntity == null)
            {
                var gameToAddEntity = mapper.Map<Game>(game);
                gameToAddEntity.Id = gameId;

                gameRepository.AddGame(companyId, gameToAddEntity);

                await gameRepository.SaveAsync();

                var dtoToReturn = mapper.Map<GameDto>(gameToAddEntity);

                return CreatedAtRoute(nameof(GetGameForCompany), new
                {
                    companyId,
                    gameId = dtoToReturn.Id
                }, dtoToReturn);
            }
            
       
            //todo 文件上传。。。

            mapper.Map(game, gameEntity);



            gameRepository.UpdateGame(gameEntity);

            await gameRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{gameId}")]//todo 文件上传。。。  IFormFile
        public async Task<IActionResult> PartiallyUpdateGameForCompany(Guid companyId, Guid gameId, JsonPatchDocument<GameUpdateDto> patchDocument)
        {
            if (!await gameRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var gameEntity = await gameRepository.GetGameAsync(companyId, gameId);

            if (gameEntity == null)
            {
                var gameDto = new GameUpdateDto();
                patchDocument.ApplyTo(gameDto, ModelState);

                if (!TryValidateModel(gameDto))
                {
                    return ValidationProblem(ModelState);
                }

                var gameToAdd = mapper.Map<Game>(gameDto);
                gameToAdd.Id = gameId;

                gameRepository.AddGame(companyId, gameToAdd);
                await gameRepository.SaveAsync();

                var dtoToReturn = mapper.Map<GameDto>(gameToAdd);

                return CreatedAtRoute(nameof(GetGameForCompany), new
                {
                    companyId,
                    gameId = dtoToReturn.Id
                }, dtoToReturn);
            }

            var dtoToPatch = mapper.Map<GameUpdateDto>(gameEntity);

            // 需要处理验证错误
            patchDocument.ApplyTo(dtoToPatch, ModelState);

            if (!TryValidateModel(dtoToPatch))
            {
                return ValidationProblem(ModelState);
            }

            mapper.Map(dtoToPatch, gameEntity);

            gameRepository.UpdateGame(gameEntity);

            await gameRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{gameId}")]
        public async Task<IActionResult> DeleteGameForCompany(Guid companyId, Guid gameId)
        {
            if (!await gameRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var gameEntity = await gameRepository.GetGameAsync(companyId, gameId);

            if (gameEntity == null)
            {
                return NotFound();
            }

            gameRepository.DeleteGame(gameEntity);

            await gameRepository.SaveAsync();

            return NoContent();
        }

        public override ActionResult ValidationProblem(ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}