using DomainCommons;
using Showcase.Domain.Entities;

namespace Showcase.Main.WebAPI.Controllers.Games.ViewModels
{
    public record GameViewModel(GameId Id, MultilingualString Title, string Introduction, Uri CoverUrl, DateTimeOffset ReleaseDate)
    {
        public static GameViewModel? Create(Game? game)
        {
            if (game == null)
            {
                return null;
            }
            return new GameViewModel(game.Id, game.Title, game.Introduction, game.CoverUrl, game.ReleaseDate);
        }

        public static GameViewModel[] Create(Game[] games)
        {
            return games.Select(g => Create(g)!).ToArray();
        }
    }
}